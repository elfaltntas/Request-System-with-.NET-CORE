import React, { useState } from 'react';
import {
    View,
    Text,
    TextInput,
    TouchableOpacity,
    StyleSheet,
    ActivityIndicator,
    Alert,
    Keyboard,
    TouchableWithoutFeedback,
} from 'react-native';
//import { db } from './FireBaseConfig';

import firebase from '@react-native-firebase/app';
import database from '@react-native-firebase/database';

// Firebase yapılandırması
const firebaseConfig = {
    apiKey: "AIzaSyBHZ7TzjKyVCmeDH7xXADSDzsfWiFnurMM",
    authDomain: "requestsystem-6fc37.firebaseapp.com",
    databaseURL: "https://requestsystem-6fc37-default-rtdb.firebaseio.com",
    projectId: "requestsystem-6fc37",
    storageBucket: "requestsystem-6fc37.appspot.com",
    messagingSenderId: "348434724882",
    appId: "1:348434724882:web:306dbc6278cd20d4ca40cb",
    measurementId: "G-2RJJKLVVCN",
};

// Firebase zaten başlatılmadıysa başlat
if (!firebase.apps.length) {
    firebase.initializeApp(firebaseConfig);
}

// Doğrudan database modülünü kullan
const db = database();

const RoleAdd: React.FC = () => {
    const [roleName, setRoleName] = useState<string>('');
    const [roleDescription, setRoleDescription] = useState<string>('');
    const [loading, setLoading] = useState<boolean>(false);

    const validateInputs = () => {
        if (!roleName.trim() || !roleDescription.trim()) {
            Alert.alert('Hata', 'Lütfen rol adı ve açıklamasını giriniz.');
            return false;
        }
        return true;
    };

    const addRoleToFirebase = async () => {
        const rolesRef = db.ref('Roles');
        const newRoleRef = rolesRef.push();

        const roleData = {
            id: newRoleRef.key,
            name: roleName,
            description: roleDescription,
            createdAt: firebase.database.ServerValue.TIMESTAMP,
        };

        try {
            await newRoleRef.set(roleData);
            Alert.alert('Başarılı', 'Rol başarıyla Firebase\'e kaydedildi.');
            setRoleName('');
            setRoleDescription('');
        } catch (error) {
            console.error('Firebase kayıt hatası:', error);
            Alert.alert('Hata', 'Rol Firebase\'e kaydedilemedi.');
        } finally {
            setLoading(false);
        }
    };

    const handleSubmit = () => {
        if (!validateInputs()) return;
        setLoading(true);
        Keyboard.dismiss();
        addRoleToFirebase();
    };

    return (
        <TouchableWithoutFeedback onPress={Keyboard.dismiss}>
            <View style={styles.container}>
                <Text style={styles.header}>Rol Ekle</Text>

                <View style={styles.formGroup}>
                    <Text style={styles.label}>Rol Adı*</Text>
                    <TextInput
                        style={styles.input}
                        value={roleName}
                        onChangeText={setRoleName}
                        placeholder="Rol adı giriniz"
                        maxLength={50}
                    />
                </View>

                <View style={styles.formGroup}>
                    <Text style={styles.label}>Rol Açıklaması*</Text>
                    <TextInput
                        style={[styles.input, styles.textArea]}
                        value={roleDescription}
                        onChangeText={setRoleDescription}
                        placeholder="Rol açıklamasını giriniz"
                        multiline
                        maxLength={200}
                    />
                </View>

                <TouchableOpacity
                    style={[styles.button, (loading || !roleName.trim() || !roleDescription.trim()) && styles.buttonDisabled]}
                    onPress={handleSubmit}
                    disabled={loading || !roleName.trim() || !roleDescription.trim()}
                >
                    {loading ? (
                        <ActivityIndicator color="#fff" />
                    ) : (
                        <Text style={styles.buttonText}>Rol Ekle</Text>
                    )}
                </TouchableOpacity>
            </View>
        </TouchableWithoutFeedback>
    );
};

const styles = StyleSheet.create({
    container: {
        flex: 1,
        padding: 16,
        backgroundColor: '#f5f5f5',
    },
    header: {
        fontSize: 24,
        fontWeight: 'bold',
        textAlign: 'center',
        color: '#333',
        marginBottom: 16,
    },
    formGroup: {
        marginBottom: 16,
    },
    label: {
        fontSize: 16,
        fontWeight: '600',
        color: '#333',
        marginBottom: 8,
    },
    input: {
        backgroundColor: '#fff',
        borderRadius: 8,
        padding: 12,
        fontSize: 16,
        elevation: 2,
    },
    textArea: {
        height: 100,
        textAlignVertical: 'top',
    },
    button: {
        backgroundColor: '#3498db',
        padding: 12,
        borderRadius: 8,
        alignItems: 'center',
    },
    buttonDisabled: {
        backgroundColor: '#bdc3c7',
    },
    buttonText: {
        fontSize: 16,
        fontWeight: '600',
        color: '#fff',
    },
});

export default RoleAdd;
