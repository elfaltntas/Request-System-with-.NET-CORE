import React, { useState } from 'react';
import {
    View,
    Text,
    TextInput,
    Button,
    StyleSheet,
    TouchableOpacity,
    FlatList,
} from 'react-native';
import { initializeApp } from 'firebase/app';
import {
    getDatabase,
    ref,
    get,
    update,
    set,
} from 'firebase/database';

// Firebase konfigürasyonu
const firebaseConfig = {
    apiKey: "AIzaSyBHZ7TzjKyVCmeDH7xXADSDzsfWiFnurMM",
    authDomain: "requestsystem-6fc37.firebaseapp.com",
    databaseURL: "https://requestsystem-6fc37-default-rtdb.firebaseio.com",
    projectId: "requestsystem-6fc37",
    storageBucket: "requestsystem-6fc37.firebasestorage.app",
    messagingSenderId: "348434724882",
    appId: "1:348434724882:web:306dbc6278cd20d4ca40cb",
    measurementId: "G-2RJJKLVVCN"
};

// Firebase başlatma
const app = initializeApp(firebaseConfig);
const database = getDatabase(app);

const RequestApproval = () => {
    const [firstName, setFirstName] = useState('');
    const [lastName, setLastName] = useState('');
    const [notes, setNotes] = useState<{ [key: string]: string }>({});
    const [requests, setRequests] = useState<any[]>([]);
    const [selectedRequest, setSelectedRequest] = useState<any | null>(null);
    const [error, setError] = useState('');
    const [success, setSuccess] = useState('');
    const [loading, setLoading] = useState(false);

    const handleFindPersonnel = async () => {
        if (!firstName.trim() || !lastName.trim()) {
            setError('Lütfen personel adı ve soyadı giriniz');
            return;
        }
        setError('');
        setSuccess('');
        setRequests([]);
        setLoading(true);

        try {
            const requestsRef = ref(database, 'Requests');
            const snapshot = await get(requestsRef);
            if (!snapshot.exists()) {
                throw new Error('Talepler bulunamadı');
            }
            const data = snapshot.val();
            const filtered = Object.keys(data)
                .map(key => ({
                    id: key,
                    ...data[key],
                    date: data[key].date
                        ? new Date(data[key].date).toLocaleDateString('tr-TR')
                        : 'Tarih yok',
                }))
                .filter(req =>
                    req.FirstName.trim().toLowerCase() === firstName.trim().toLowerCase() &&
                    req.LastName.trim().toLowerCase() === lastName.trim().toLowerCase()
                );
            if (filtered.length === 0) {
                throw new Error('Verilen ad ve soyada ait talep bulunamadı');
            }
            setRequests(filtered);
            setSuccess(`${firstName} ${lastName} personeline ait talepler listelendi`);
        } catch (err: any) {
            setError(err.message);
        } finally {
            setLoading(false);
        }
    };

    const handleUpdateStatus = async (
        requestId: string,
        action: 'Completed' | 'Cancelled'
    ) => {
        const status = action === 'Completed' ? 'Onaylandı' : 'Reddedildi';
        try {
            // 1) Mevcut Requests kaydını güncelle
            const requestRef = ref(database, `Requests/${requestId}`);
            await update(requestRef, {
                status,
                adminNotes: notes[requestId] || '',
                updatedAt: new Date().toISOString(),
            });

            // 2) RequestProcess altında bir log kaydı oluştur
            const processRef = ref(database, `RequestProcess/${requestId}`);
            await set(processRef, {
                requestId,
                status,
                adminNotes: notes[requestId] || '',
                processedAt: new Date().toISOString(),
                processedBy: `${firstName} ${lastName}`,
            });

            setSuccess(`Talep başarıyla güncellendi ve RequestProcess’a kaydedildi: ${status}`);
            await handleFindPersonnel();
            setSelectedRequest(null);
        } catch (err: any) {
            setError(`Hata: ${err.message}`);
        }
    };

    const renderRequest = ({ item }: { item: any }) => (
        <TouchableOpacity
            style={styles.requestCard}
            onPress={() => setSelectedRequest(item)}
        >
            <Text style={styles.requestTitle}>Talep ID: {item.id}</Text>
            <Text>Durum: {item.status}</Text>
            <Text>Açıklama: {item.description}</Text>
            <Text>Tarih: {item.date}</Text>
        </TouchableOpacity>
    );

    return (
        <View style={styles.container}>
            <Text style={styles.header}>Talep Onaylama Sistemi</Text>
            <View style={styles.inputContainer}>
                <TextInput
                    style={styles.input}
                    placeholder="Personel Adı"
                    value={firstName}
                    onChangeText={setFirstName}
                />
                <TextInput
                    style={styles.input}
                    placeholder="Personel Soyadı"
                    value={lastName}
                    onChangeText={setLastName}
                />
                <Button
                    title={loading ? 'Aranıyor...' : 'Personel Taleplerini Getir'}
                    onPress={handleFindPersonnel}
                    disabled={loading}
                />
            </View>
            {!!error && <Text style={styles.error}>{error}</Text>}
            {!!success && <Text style={styles.success}>{success}</Text>}

            <FlatList
                data={requests}
                keyExtractor={i => i.id}
                renderItem={renderRequest}
            />

            {selectedRequest && (
                <View style={styles.detailsContainer}>
                    <Text style={styles.detailTitle}>Talep Detayları</Text>
                    <Text>Talep ID: {selectedRequest.id}</Text>
                    <Text>Durum: {selectedRequest.status}</Text>
                    <Text>Açıklama: {selectedRequest.description}</Text>
                    <Text>Tarih: {selectedRequest.date}</Text>

                    <TextInput
                        style={styles.textArea}
                        placeholder="İşlem notu giriniz..."
                        value={notes[selectedRequest.id] || ''}
                        onChangeText={text =>
                            setNotes(prev => ({ ...prev, [selectedRequest.id]: text }))
                        }
                        multiline
                    />

                    <View style={styles.buttonsContainer}>
                        <Button
                            title="Onayla"
                            color="#4caf50"
                            onPress={() => handleUpdateStatus(selectedRequest.id, 'Completed')}
                        />
                        <Button
                            title="İptal Et"
                            color="#f44336"
                            onPress={() => handleUpdateStatus(selectedRequest.id, 'Cancelled')}
                        />
                    </View>
                </View>
            )}
        </View>
    );
};

const styles = StyleSheet.create({
    container: { flex: 1, padding: 16, backgroundColor: '#f9f9f9' },
    header: {
        fontSize: 28,
        fontWeight: 'bold',
        textAlign: 'center',
        marginBottom: 24,
        color: '#333',
    },
    inputContainer: {
        marginBottom: 24,
        padding: 16,
        backgroundColor: '#fff',
        borderRadius: 12,
        elevation: 2,
    },
    input: {
        borderWidth: 1,
        borderColor: '#ddd',
        padding: 12,
        borderRadius: 8,
        backgroundColor: '#fefefe',
        fontSize: 16,
        marginBottom: 12,
    },
    error: { color: 'red', textAlign: 'center', marginVertical: 8 },
    success: { color: 'green', textAlign: 'center', marginVertical: 8 },
    requestCard: {
        padding: 16,
        backgroundColor: '#fff',
        marginBottom: 12,
        borderRadius: 12,
        elevation: 3,
    },
    requestTitle: { fontWeight: '600', fontSize: 18, color: '#444' },
    detailsContainer: {
        padding: 20,
        backgroundColor: '#fff',
        borderRadius: 12,
        elevation: 3,
        marginTop: 16,
    },
    detailTitle: { fontSize: 20, fontWeight: 'bold', marginBottom: 12 },
    textArea: {
        height: 100,
        borderWidth: 1,
        borderColor: '#ddd',
        borderRadius: 8,
        padding: 12,
        marginVertical: 16,
        backgroundColor: '#fefefe',
        textAlignVertical: 'top',
    },
    buttonsContainer: { flexDirection: 'row', justifyContent: 'space-between' },
});

export default RequestApproval;
