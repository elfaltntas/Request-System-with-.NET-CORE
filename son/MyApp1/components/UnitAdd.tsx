import React, { useState } from "react";
import { View, Text, TextInput, Button, StyleSheet, Alert } from "react-native";
import { initializeApp } from "firebase/app";
import { getDatabase, ref, push } from "firebase/database";

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

const UnitAddForm: React.FC = () => {
    const [formData, setFormData] = useState({
        unitId: "",
        name: "",
        parentUnitId: "",
        roleId: "",
    });

    const handleChange = (name: string, value: string) => {
        setFormData({ ...formData, [name]: value });
    };

    const handleSubmit = async () => {
        try {
            const unitRef = ref(database, "units");
            await push(unitRef, formData);
            Alert.alert("Başarılı", "Birim başarıyla kaydedildi!");

            // Formu temizle
            setFormData({
                unitId: "",
                name: "",
                parentUnitId: "",
                roleId: "",
            });
        } catch (error) {
            Alert.alert("Hata", "Birimi kaydederken bir sorun oluştu!");
            console.error("Firebase Error:", error);
        }
    };

    return (
        <View style={styles.container}>
            <Text style={styles.title}>Birim Ekle</Text>

            <Text style={styles.label}>Birim ID:</Text>
            <TextInput
                style={styles.input}
                value={formData.unitId}
                onChangeText={(value) => handleChange("unitId", value)}
                placeholder="Birim ID giriniz"
            />

            <Text style={styles.label}>Birim Adı:</Text>
            <TextInput
                style={styles.input}
                value={formData.name}
                onChangeText={(value) => handleChange("name", value)}
                placeholder="Birim adı giriniz"
            />

            <Text style={styles.label}>Üst Birim ID:</Text>
            <TextInput
                style={styles.input}
                value={formData.parentUnitId}
                onChangeText={(value) => handleChange("parentUnitId", value)}
                placeholder="Üst birim ID giriniz"
            />

            <Text style={styles.label}>Rol ID:</Text>
            <TextInput
                style={styles.input}
                value={formData.roleId}
                onChangeText={(value) => handleChange("roleId", value)}
                placeholder="Rol ID giriniz"
            />

            <Button title="Kaydet" onPress={handleSubmit} />
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
        marginBottom: 20,
        textAlign: "center",
    },
    label: {
        fontSize: 16,
        marginBottom: 8,
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
});

export default UnitAddForm;
