import React, { useState, useEffect } from 'react';
import {
    View,
    Text,
    StyleSheet,
    ScrollView,
    ActivityIndicator,
} from 'react-native';
import { Picker } from '@react-native-picker/picker';
import { initializeApp } from 'firebase/app';
import {
    getDatabase,
    ref,
    get,
} from 'firebase/database';
import { format } from 'date-fns';
import { tr } from 'date-fns/locale';

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

// Firebase'i başlat
const app = initializeApp(firebaseConfig);
const db = getDatabase(app);

type Personnel = { personelId: string; firstName: string; lastName: string };

const PersonnelRequestView = () => {
    const [selectedPersonnel, setSelectedPersonnel] = useState<Personnel | null>(null);
    const [personnels, setPersonnels] = useState<Personnel[]>([]);
    const [requests, setRequests] = useState<any[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

    // 1) Personel listesini al
    useEffect(() => {
        (async () => {
            try {
                const snap = await get(ref(db, 'Users'));
                if (!snap.exists()) return setPersonnels([]);
                const data = snap.val();
                setPersonnels(Object.keys(data).map(key => ({
                    personelId: key,
                    firstName: data[key].firstName,
                    lastName: data[key].lastName,
                })));
            } catch (e) {
                console.error(e);
                setError('Personeller alınamadı.');
            }
        })();
    }, []);

    // 2) Seçilen kişiye ait talepleri ve process durumlarını getir
    useEffect(() => {
        if (!selectedPersonnel) {
            setRequests([]);
            return;
        }

        (async () => {
            setLoading(true);
            setError('');
            try {
                // a) Tüm Requests
                const snap = await get(ref(db, 'Requests'));
                const data = snap.exists() ? snap.val() : {};

                // b) İsim/soyisim filtresi
                const filteredIds = Object.keys(data).filter(key => {
                    const r = data[key];
                    return (
                        r.FirstName?.trim().toLowerCase() === selectedPersonnel.firstName.trim().toLowerCase() &&
                        r.LastName?.trim().toLowerCase() === selectedPersonnel.lastName.trim().toLowerCase()
                    );
                });

                // c) Her kayıt için RequestProcess bilgilerini çek
                const merged = await Promise.all(filteredIds.map(async requestId => {
                    const r = data[requestId];
                    // process snapshot
                    const processSnap = await get(ref(db, `RequestProcess/${requestId}`));
                    const p = processSnap.exists() ? processSnap.val() : {};

                    return {
                        requestId,
                        title: r.title,
                        description: r.description,
                        // Process tablosundan öncelikle al, yoksa Requests'ten al
                        status: p.status ?? r.status ?? 'Bilinmeyen',
                        adminNotes: p.adminNotes ?? p.notes ?? '',
                        processedAt: p.processedAt ?? '',
                        processedBy: p.processedBy ?? '',
                        createdDate: r.createdDate,
                        modifiedDate: r.modifiedDate,
                    };
                }));

                setRequests(merged);
            } catch (e) {
                console.error(e);
                setError('Talepler alınırken hata oluştu.');
                setRequests([]);
            } finally {
                setLoading(false);
            }
        })();
    }, [selectedPersonnel]);

    const formatDate = (d: string) => {
        if (!d || d.startsWith('0001')) return 'Tarih yok';
        try {
            return format(new Date(d), 'dd.MM.yyyy HH:mm', { locale: tr });
        } catch {
            return d;
        }
    };

    const getStatusColor = (s: string) => {
        switch (s) {
            case 'completed': return '#28a745';
            case 'pending': return '#ffc107';
            /*case 'cancelled':*/
            case 'cancelled': return '#dc3545';
            default: return '#6c757d';
        }
    };

    return (
        <ScrollView style={styles.container}>
            <Text style={styles.header}>Personel Talep Görüntüleme</Text>

            <View style={styles.card}>
                <Text style={styles.label}>Personel Seçin:</Text>
                <Picker
                    selectedValue={selectedPersonnel?.personelId || ''}
                    onValueChange={val => {
                        setSelectedPersonnel(personnels.find(x => x.personelId === val) || null);
                    }}
                    style={styles.picker}
                >
                    <Picker.Item label="-- SECINIZ --" value="" />
                    {personnels.map(p => (
                        <Picker.Item
                            key={p.personelId}
                            label={`${p.firstName} ${p.lastName}`}
                            value={p.personelId}
                        />
                    ))}
                </Picker>
            </View>

            {loading && <ActivityIndicator size="large" color="#007bff" style={styles.loader} />}
            {!!error && <Text style={styles.errorText}>{error}</Text>}

            <View style={styles.card}>
                <Text style={styles.sectionHeader}>Personel Talepleri</Text>

                {!selectedPersonnel ? (
                    <Text style={styles.infoText}>Lütfen personel seçin.</Text>
                ) : requests.length === 0 ? (
                    <Text style={styles.infoText}>Bu personele ait talep bulunmuyor.</Text>
                ) : (
                    requests.map(r => (
                        <View key={r.requestId} style={styles.requestItem}>
                            <View style={styles.row}>
                                <Text style={styles.label}>Durum:</Text>
                                <Text style={[styles.value, { color: getStatusColor(r.status) }]}>
                                    {r.status}
                                </Text>
                            </View>
                            <View style={styles.row}>
                                <Text style={styles.label}>Not:</Text>
                                <Text style={styles.value}>{r.adminNotes || '—'}</Text>
                            </View>
                            <View style={styles.row}>
                                <Text style={styles.label}>İşlemci:</Text>
                                <Text style={styles.value}>{r.processedBy || '—'}</Text>
                            </View>
                            <View style={styles.row}>
                                <Text style={styles.label}>İşlem Zamanı:</Text>
                                <Text style={styles.value}>{formatDate(r.processedAt)}</Text>
                            </View>
                            <View style={styles.row}>
                                <Text style={styles.label}>Başlık:</Text>
                                <Text style={styles.value}>{r.title || '-'}</Text>
                            </View>
                            <View style={styles.row}>
                                <Text style={styles.label}>Açıklama:</Text>
                                <Text style={styles.value}>{r.description || '-'}</Text>
                            </View>
                            <View style={styles.row}>
                                <Text style={styles.label}>Oluşturma:</Text>
                                <Text style={styles.value}>{formatDate(r.createdDate)}</Text>
                            </View>
                            <View style={styles.row}>
                                <Text style={styles.label}>Güncelleme:</Text>
                                <Text style={styles.value}>{formatDate(r.modifiedDate)}</Text>
                            </View>
                        </View>
                    ))
                )}
            </View>
        </ScrollView>
    );
};

const styles = StyleSheet.create({
    container: { flex: 1, backgroundColor: '#f7f9fc', padding: 16 },
    header: { fontSize: 28, fontWeight: '700', textAlign: 'center', marginBottom: 24, color: '#1e1e1e' },
    card: { backgroundColor: '#fff', borderRadius: 12, padding: 20, marginBottom: 20, elevation: 5 },
    label: { fontWeight: '600', fontSize: 16, color: '#4a4a4a', marginBottom: 8 },
    picker: { backgroundColor: '#e9ecef', borderRadius: 8, marginBottom: 16 },
    loader: { marginVertical: 20 },
    errorText: { color: 'red', textAlign: 'center', marginBottom: 16 },
    sectionHeader: { fontSize: 20, fontWeight: '700', marginBottom: 16, color: '#333' },
    requestItem: { backgroundColor: '#fff', borderRadius: 12, padding: 16, marginBottom: 16, elevation: 3 },
    row: { flexDirection: 'row', justifyContent: 'space-between', marginBottom: 8 },
    value: { fontSize: 16, color: '#333' },
    infoText: { textAlign: 'center', color: '#6c757d', marginVertical: 20, fontSize: 16 },
});

export default PersonnelRequestView;
