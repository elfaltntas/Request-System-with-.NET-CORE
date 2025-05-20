import React, { useState, useEffect } from 'react';
import {
    View,
    Text,
    StyleSheet,
    TouchableOpacity,
    ActivityIndicator,
    Alert,
    ScrollView
} from 'react-native';
import { Picker } from '@react-native-picker/picker';
import { initializeApp } from 'firebase/app';
import {
    getDatabase,
    ref,
    get,
    update
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
// Firebase'i başlat
const app = initializeApp(firebaseConfig);
const db = getDatabase(app);

type Personnel = {
    personelId: string;
    firstName: string;
    lastName: string;
    isPersonel: boolean;
    terminatedAt?: string;
};

const PersonnelList: React.FC<{ navigation: any }> = ({ navigation }) => {
    const [personnels, setPersonnels] = useState<Personnel[]>([]);
    const [selectedId, setSelectedId] = useState<string>('');
    const [loading, setLoading] = useState(false);
    const [terminating, setTerminating] = useState(false);

    // 1) Aktif personelleri Firebase'den oku
    useEffect(() => {
        (async () => {
            setLoading(true);
            try {
                const snap = await get(ref(db, 'Users'));
                if (!snap.exists()) {
                    setPersonnels([]);
                    return;
                }
                const data = snap.val();
                const list: Personnel[] = Object.keys(data).map(key => ({
                    personelId: key,
                    firstName: data[key].firstName,
                    lastName: data[key].lastName,
                    isPersonel: data[key].isPersonel ?? true,
                    terminatedAt: data[key].terminatedAt
                }))
                    // sadece halen personel olanları listele
                    .filter(p => p.isPersonel);
                setPersonnels(list);
            } catch (e) {
                console.error(e);
                Alert.alert('Hata', 'Personel listesi alınamadı.');
            } finally {
                setLoading(false);
            }
        })();
    }, []);

    // 2) İşten çıkarma onayı
    const confirmTerminate = () => {
        if (!selectedId) {
            Alert.alert('Uyarı', 'Önce bir personel seçin.');
            return;
        }
        const p = personnels.find(x => x.personelId === selectedId);
        if (!p) return;
        Alert.alert(
            'İşten Çıkar',
            `${p.firstName} ${p.lastName} işten çıkarılsın mı?`,
            [
                { text: 'İptal', style: 'cancel' },
                { text: 'Evet', onPress: handleTerminate, style: 'destructive' }
            ]
        );
    };

    // 3) Firebase üzerinde isPersonel = false ve terminatedAt ekle
    const handleTerminate = async () => {
        setTerminating(true);
        try {
            const updates: Partial<Personnel> = {
                isPersonel: false,
                terminatedAt: new Date().toISOString()
            };
            await update(ref(db, `Users/${selectedId}`), updates);

            // UI güncelle
            setPersonnels(prev => prev.filter(p => p.personelId !== selectedId));
            setSelectedId('');
            Alert.alert('Başarılı', 'Personel işten çıkarıldı.');
        } catch (e) {
            console.error(e);
            Alert.alert('Hata', 'İşten çıkarma işlemi başarısız.');
        } finally {
            setTerminating(false);
        }
    };

    return (
        <ScrollView contentContainerStyle={styles.container}>
            <Text style={styles.header}>Personel İşten Çıkarma</Text>

            {loading && <ActivityIndicator size="large" color="#0000ff" style={styles.loading} />}

            <View style={styles.formGroup}>
                <Text style={styles.label}>Personel Seçin:</Text>
                <View style={styles.pickerContainer}>
                    <Picker
                        selectedValue={selectedId}
                        onValueChange={setSelectedId}
                        style={styles.picker}
                    >
                        <Picker.Item label="-- Seçin --" value="" />
                        {personnels.map(p => (
                            <Picker.Item
                                key={p.personelId}
                                label={`${p.firstName} ${p.lastName}`}
                                value={p.personelId}
                            />
                        ))}
                    </Picker>
                </View>
            </View>

            <TouchableOpacity
                onPress={confirmTerminate}
                disabled={!selectedId || terminating}
                style={[
                    styles.button,
                    { backgroundColor: selectedId ? '#d32f2f' : '#ccc' }
                ]}
            >
                {terminating
                    ? <ActivityIndicator color="#fff" />
                    : <Text style={styles.buttonText}>İŞTEN ÇIKAR</Text>
                }
            </TouchableOpacity>

            <TouchableOpacity
                onPress={() => navigation.navigate('Dashboard')}
                style={styles.navigationButton}
            >
                <Text style={styles.navigationButtonText}>Dashboard'a Dön</Text>
            </TouchableOpacity>
        </ScrollView>
    );
};

const styles = StyleSheet.create({
    container: { flexGrow: 1, padding: 20, backgroundColor: '#f5f5f5' },
    header: { fontSize: 22, fontWeight: 'bold', textAlign: 'center', marginBottom: 20 },
    loading: { marginVertical: 20 },
    formGroup: { marginBottom: 20 },
    label: { fontSize: 16, marginBottom: 8, fontWeight: '600' },
    pickerContainer: {
        borderWidth: 1, borderColor: '#ccc', borderRadius: 5, backgroundColor: '#fff'
    },
    picker: { height: 50, width: '100%' },
    button: {
        padding: 15, borderRadius: 5, alignItems: 'center',
        marginBottom: 10, height: 50, justifyContent: 'center'
    },
    buttonText: { color: '#fff', fontSize: 16, fontWeight: 'bold' },
    navigationButton: {
        padding: 10, backgroundColor: '#007BFF', borderRadius: 5, alignItems: 'center'
    },
    navigationButtonText: { color: '#fff', fontSize: 16, fontWeight: 'bold' }
});

export default PersonnelList;
