import React, { useState, useEffect } from 'react';
import {
    View,
    Text,
    Button,
    StyleSheet,
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

// Firebase config (sizin config'inizle değiştirin)
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

const app = initializeApp(firebaseConfig);
const db = getDatabase(app);

type Unit = {
    unitId: string;
    name: string;
    parentUnitId?: string;
    parentDepartmentId?: string;
};

const UnitMove: React.FC = () => {
    const [units, setUnits] = useState<Unit[]>([]);
    const [selectedUnit, setSelectedUnit] = useState<string>('');
    const [selectedParent, setSelectedParent] = useState<string>('');
    const [loading, setLoading] = useState(false);
    const [fetching, setFetching] = useState(true);
    const [successMessage, setSuccessMessage] = useState('');

    useEffect(() => {
        (async () => {
            setFetching(true);
            try {
                const snap = await get(ref(db, 'units'));
                if (!snap.exists()) {
                    setUnits([]);
                } else {
                    const data = snap.val();
                    setUnits(
                        Object.keys(data).map(key => ({
                            unitId: key,
                            name: data[key].name,
                            parentUnitId: data[key].parentUnitId,
                            parentDepartmentId: data[key].parentDepartmentId
                        }))
                    );
                }
            } catch (e) {
                console.error(e);
                Alert.alert('Hata', 'Birimler yüklenirken hata oluştu.');
            } finally {
                setFetching(false);
            }
        })();
    }, []);

    const handleSubmit = async () => {
        if (!selectedUnit || !selectedParent) {
            Alert.alert('Hata', 'Lütfen tüm alanları doldurun.');
            return;
        }
        setLoading(true);
        setSuccessMessage('');
        try {
            // Seçilen departmanı units içinden bul
            const parentDept = units.find(u => u.unitId === selectedParent);
            if (!parentDept) {
                Alert.alert('Hata', 'Seçilen departman bulunamadı.');
                setLoading(false);
                return;
            }

            // Departmanın unitId'sini alıyoruz
            const parentUnitId = parentDept.unitId;

            // Güncelleme objesi
            const updates: Record<string, any> = {};

            updates[`units/${selectedUnit}/parentUnitId`] = parentUnitId;
            updates[`units/${selectedUnit}/parentDepartmentId`] = selectedParent;
            updates[`departments/${selectedParent}/units/${selectedUnit}`] = true;

            await update(ref(db), updates);

            setSuccessMessage('Birim başarıyla taşındı!');
            setSelectedUnit('');
            setSelectedParent('');
        } catch (e) {
            console.error(e);
            Alert.alert('Hata', 'Taşıma işlemi başarısız oldu.');
        } finally {
            setLoading(false);
        }
    };

    if (fetching) {
        return (
            <View style={styles.center}>
                <ActivityIndicator size="large" color="#007bff" />
                <Text>Birimler yükleniyor...</Text>
            </View>
        );
    }

    return (
        <ScrollView contentContainerStyle={styles.container}>
            <Text style={styles.title}>Birim Taşıma İşlemi</Text>

            <View style={styles.formGroup}>
                <Text style={styles.label}>Taşınacak Birim</Text>
                <View style={styles.pickerContainer}>
                    <Picker
                        selectedValue={selectedUnit}
                        onValueChange={setSelectedUnit}
                        style={styles.picker}
                    >
                        <Picker.Item label="Birim seçiniz" value="" />
                        {units.map(u => (
                            <Picker.Item
                                key={u.unitId}
                                label={`${u.name} (ID: ${u.unitId})`}
                                value={u.unitId}
                            />
                        ))}
                    </Picker>
                </View>
            </View>

            <View style={styles.formGroup}>
                <Text style={styles.label}>Yeni Üst Birim (Department)</Text>
                <View style={styles.pickerContainer}>
                    <Picker
                        selectedValue={selectedParent}
                        onValueChange={setSelectedParent}
                        style={styles.picker}
                    >
                        <Picker.Item label="Üst birim seçiniz" value="" />
                        {units.map(u => (
                            <Picker.Item
                                key={u.unitId}
                                label={`${u.name} (ID: ${u.unitId})`}
                                value={u.unitId}
                            />
                        ))}
                    </Picker>
                </View>
            </View>

            {successMessage ? (
                <View style={styles.successBox}>
                    <Text style={styles.successText}>{successMessage}</Text>
                </View>
            ) : null}

            <Button
                title={loading ? 'İşleniyor...' : 'Birimi Taşı'}
                onPress={handleSubmit}
                color="#007bff"
                disabled={loading || !selectedUnit || !selectedParent}
            />
        </ScrollView>
    );
};

const styles = StyleSheet.create({
    container: {
        flexGrow: 1,
        padding: 16,
        backgroundColor: '#f5f5f5',
    },
    center: {
        flex: 1,
        justifyContent: 'center',
        alignItems: 'center',
    },
    title: {
        fontSize: 20,
        fontWeight: 'bold',
        marginBottom: 16,
        textAlign: 'center',
    },
    formGroup: {
        marginBottom: 16,
    },
    label: {
        fontSize: 16,
        marginBottom: 8,
    },
    pickerContainer: {
        borderWidth: 1,
        borderColor: '#ccc',
        borderRadius: 8,
        backgroundColor: '#fff',
    },
    picker: {
        height: 50,
        width: '100%',
    },
    successBox: {
        backgroundColor: '#d4edda',
        padding: 10,
        borderRadius: 5,
        marginBottom: 16,
    },
    successText: {
        color: '#155724',
        textAlign: 'center',
    },
});

export default UnitMove;
