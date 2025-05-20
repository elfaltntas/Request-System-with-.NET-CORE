import React, { useState } from 'react';
import {
    View,
    Text,
    TextInput,
    TouchableOpacity,
    StyleSheet,
    ActivityIndicator,
 
} from 'react-native';
import { initializeApp } from 'firebase/app';
import { getDatabase, ref, get } from 'firebase/database';

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
// Firebase’i başlat
const app = initializeApp(firebaseConfig);
const db = getDatabase(app);

const RequestStatus: React.FC = () => {
    const [requestId, setRequestId] = useState<string>('');
    const [statusData, setStatusData] = useState<{ statusDescription: string; Notes: string }>({
        statusDescription: '',
        Notes: ''
    });
    const [loading, setLoading] = useState<boolean>(false);
    const [error, setError] = useState<string>('');

    const fetchRequestStatus = async () => {
        if (!requestId.trim()) {
            setError('Lütfen bir Talep ID giriniz.');
            setStatusData({ statusDescription: '', Notes: '' });
            return;
        }

        setLoading(true);
        setError('');
        setStatusData({ statusDescription: '', Notes: '' });

        try {
            // Firebase’den RequestProcess/{requestId} oku
            const snap = await get(ref(db, `RequestProcess/${requestId.trim()}`));
            if (!snap.exists()) {
                setError('Talep bulunamadı. Lütfen geçerli bir Talep ID giriniz.');
            } else {
                const data = snap.val();
                // statusDescription veya status, Notes veya adminNotes
                const statusDescription: string =
                    data.statusDescription ??
                    data.statusText ??
                    data.status ??
                    'Bilinmeyen Durum';
                const Notes: string = data.Notes ?? data.adminNotes ?? '';
                setStatusData({ statusDescription, Notes });
            }
        } catch (e: any) {
            console.error(e);
            setError('Bir hata oluştu: ' + e.message);
        } finally {
            setLoading(false);
        }
    };

    return (
        <View style={styles.container}>
            <Text style={styles.header}>Talep Durumu Görüntüle</Text>

            <View style={styles.card}>
                <TextInput
                    style={styles.input}
                    placeholder="Talep ID giriniz"
                    value={requestId}
                    onChangeText={setRequestId}
                    onSubmitEditing={fetchRequestStatus}
                />

                <TouchableOpacity
                    style={[styles.button, loading && styles.buttonDisabled]}
                    onPress={fetchRequestStatus}
                    disabled={loading}
                >
                    {loading
                        ? <ActivityIndicator color="#fff" />
                        : <Text style={styles.buttonText}>Durumu Sorgula</Text>
                    }
                </TouchableOpacity>
            </View>

            {!!error && (
                <View style={styles.alert}>
                    <Text style={styles.alertText}>{error}</Text>
                </View>
            )}

            {statusData.statusDescription !== '' && !error && (
                <View style={styles.statusContainer}>
                    <Text style={styles.statusText}>
                        <Text style={styles.bold}>Talep Durumu:</Text> {statusData.statusDescription}
                    </Text>
                    {statusData.Notes !== '' && (
                        <Text style={styles.statusText}>
                            <Text style={styles.bold}>Not:</Text> {statusData.Notes}
                        </Text>
                    )}
                </View>
            )}
        </View>
    );
};

const styles = StyleSheet.create({
    container: { flex: 1, padding: 16, backgroundColor: '#fff' },
    header: { fontSize: 24, fontWeight: 'bold', textAlign: 'center', marginBottom: 20 },
    card: {
        padding: 16,
        borderRadius: 8,
        backgroundColor: '#f5f5f5',
        marginBottom: 20,
        elevation: 3,
    },
    input: {
        height: 50,
        borderColor: '#ccc',
        borderWidth: 1,
        borderRadius: 8,
        marginBottom: 16,
        paddingHorizontal: 16,
        fontSize: 16,
    },
    button: {
        backgroundColor: '#007bff',
        paddingVertical: 12,
        borderRadius: 8,
        alignItems: 'center',
    },
    buttonDisabled: {
        backgroundColor: '#999',
    },
    buttonText: {
        color: '#fff',
        fontSize: 16,
    },
    alert: {
        backgroundColor: '#f8d7da',
        padding: 10,
        borderRadius: 8,
        marginBottom: 16,
    },
    alertText: {
        color: '#721c24',
        fontSize: 16,
    },
    statusContainer: {
        backgroundColor: '#e9ecef',
        padding: 16,
        borderRadius: 8,
    },
    statusText: {
        fontSize: 16,
        marginBottom: 8,
    },
    bold: {
        fontWeight: 'bold',
    },
});

export default RequestStatus;
