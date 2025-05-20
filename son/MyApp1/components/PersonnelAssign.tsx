import React, { useState, useEffect } from "react";
import { View, Text, TextInput, Button, StyleSheet, Alert, ActivityIndicator} from "react-native";
import { initializeApp } from "firebase/app";
import { Picker } from '@react-native-picker/picker';
import { getDatabase, ref, push, get } from "firebase/database";

// Firebase Configuration
const firebaseConfig = {
    apiKey: "AIzaSyBHZ7TzjKyVCmeDH7xXADSDzsfWiFnurMM",
    authDomain: "requestsystem-6fc37.firebaseapp.com",
    databaseURL: "https://requestsystem-6fc37-default-rtdb.firebaseio.com",
    projectId: "requestsystem-6fc37",
    storageBucket: "requestsystem-6fc37.firebasestorage.app",
    messagingSenderId: "348434724882",
    appId: "1:348434724882:web:306dbc6278cd20d4ca40cb",
    measurementId: "G-2RJJKLVVCN",
};

// Initialize Firebase
const app = initializeApp(firebaseConfig);
const database = getDatabase(app);

const PersonnelAssign: React.FC = () => {
    const [firstName, setFirstName] = useState("");
    const [lastName, setLastName] = useState("");
    const [units, setUnits] = useState<Array<{ id: string; name: string }>>([]);
    const [selectedUnitId, setSelectedUnitId] = useState("");
    const [personnelId, setPersonnelId] = useState("");
    const [loading, setLoading] = useState(false);

    // Fetch units from Firebase
    useEffect(() => {
        const fetchUnits = async () => {
            try {
                const dbRef = ref(database, "units");
                const snapshot = await get(dbRef);
                if (snapshot.exists()) {
                    const data = snapshot.val();
                    const unitList = Object.keys(data).map((key) => ({
                        id: key,
                        name: data[key].name,
                    }));
                    setUnits(unitList);
                } else {
                    Alert.alert("Bilgi", "Kayıtlı birim bulunamadı.");
                }
            } catch (error) {
                console.error("Hata:", error);
                Alert.alert("Hata", "Birimler alınırken bir hata oluştu.");
            }
        };

        fetchUnits();
    }, []);

    // Save personnel assignment to Firebase
    const assignPersonnelToUnit = async () => {
        if (!firstName.trim() || !lastName.trim() || !selectedUnitId.trim()) {
            Alert.alert("Hata", "Tüm alanları doldurduğunuzdan emin olun.");
            return;
        }

        setLoading(true);
        try {
            const personnelRef = ref(database, "personnelAssignments");
            await push(personnelRef, {
                firstName,
                lastName,
                personnelId,
                unitId: selectedUnitId,
            });
            Alert.alert("Başarılı", "Personel birime başarıyla atandı!");
            setFirstName("");
            setLastName("");
            setPersonnelId("");
            setSelectedUnitId("");
        } catch (error) {
            console.error("Firebase Error:", error);
            Alert.alert("Hata", "Personel atanırken bir hata oluştu.");
        } finally {
            setLoading(false);
        }
    };

    return (
        <View style={styles.container}>
            <Text style={styles.title}>Personel Birim Atama</Text>

            <TextInput
                style={styles.input}
                placeholder="Ad giriniz"
                value={firstName}
                onChangeText={setFirstName}
            />
            <TextInput
                style={styles.input}
                placeholder="Soyad giriniz"
                value={lastName}
                onChangeText={setLastName}
            />

            <Text style={styles.label}>Birim Seç:</Text>
            <Picker
                selectedValue={selectedUnitId}
                onValueChange={(value) => setSelectedUnitId(value)}
                style={styles.picker}
            >
                <Picker.Item label="Birim seçiniz" value="" />
                {units.map((unit) => (
                    <Picker.Item key={unit.id} label={unit.name} value={unit.id} />
                ))}
            </Picker>

            <Button
                title={loading ? "Atama Yapılıyor..." : "Atama Yap"}
                onPress={assignPersonnelToUnit}
                disabled={loading}
            />
            {loading && <ActivityIndicator size="large" color="#0000ff" />}
        </View>
    );
};

const styles = StyleSheet.create({
    container: {
        flex: 1,
        padding: 16,
        backgroundColor: "#f8f9fa",
    },
    title: {
        fontSize: 24,
        fontWeight: "bold",
        textAlign: "center",
        marginBottom: 16,
    },
    input: {
        height: 40,
        borderColor: "#ccc",
        borderWidth: 1,
        borderRadius: 8,
        paddingHorizontal: 8,
        marginBottom: 16,
        backgroundColor: "#fff",
    },
    label: {
        fontSize: 16,
        marginBottom: 8,
    },
    picker: {
        height: 40,
        marginBottom: 16,
    },
});

export default PersonnelAssign;
