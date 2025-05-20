import React from 'react';
import { View, TextInput, StyleSheet, Button, TouchableOpacity, Text, Alert } from 'react-native';
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

const UserRegistrationForm: React.FC = ({ navigation }: any) => {
    const [firstName, setFirstName] = React.useState('');
    const [lastName, setLastName] = React.useState('');
    const [userName, setUserName] = React.useState('');
    const [password, setPassword] = React.useState('');
    const [email, setEmail] = React.useState('');
    const [phoneNumber, setPhoneNumber] = React.useState('');
    const [isPersonel, setIsPersonel] = React.useState(false);

    const validateForm = () => {
        const nameRegex = /^[A-ZÇĞİÖŞÜ][a-zçğıöşü]+$/;
        const passwordRegex = /^(?=.*[A-Z])(?=.*[!@#$%^&*()_+=\-]).{6,}$/;
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

        if (!nameRegex.test(firstName)) {
            Alert.alert('Hata', 'Ad büyük harfle başlamalı ve sadece harflerden oluşmalı.');
            return false;
        }

        if (!nameRegex.test(lastName)) {
            Alert.alert('Hata', 'Soyad büyük harfle başlamalı ve sadece harflerden oluşmalı.');
            return false;
        }

        if (!passwordRegex.test(password)) {
            Alert.alert(
                'Hata',
                'Şifre en az bir büyük harf ve bir özel karakter içermeli ve en az 6 karakter uzunluğunda olmalıdır.'
            );
            return false;
        }

        if (!emailRegex.test(email)) {
            Alert.alert('Hata', 'Geçerli bir e-posta adresi giriniz.');
            return false;
        }

        return true;
    };

    const handleRegister = async () => {
        if (!validateForm()) return;

        const formData = {
            firstName,
            lastName,
            userName,
            password,
            email,
            phoneNumber,
            isPersonel,
        };

        try {
            // Firebase Realtime Database'e kullanıcı kaydını ekliyoruz
            const userRef = database().ref('Users').push();  // yeni bir user ID'si oluşturur
            await userRef.set(formData);

            Alert.alert('Başarılı', 'Kullanıcı başarıyla kaydedildi!');
            navigation.goBack(); // Kayıt başarılı olursa geri dön
        } catch (error) {
            console.error('Kayıt Hatası:', error);
            Alert.alert('Hata', error instanceof Error ? error.message : 'Bilinmeyen bir hata oluştu');
        }
    };

    return (
        <View style={styles.container}>
            <TextInput
                style={styles.input}
                placeholder="Ad"
                value={firstName}
                onChangeText={setFirstName}
            />
            <TextInput
                style={styles.input}
                placeholder="Soyad"
                value={lastName}
                onChangeText={setLastName}
            />
            <TextInput
                style={styles.input}
                placeholder="Kullanıcı Adı"
                value={userName}
                onChangeText={setUserName}
            />
            <TextInput
                style={styles.input}
                placeholder="Şifre"
                secureTextEntry
                value={password}
                onChangeText={setPassword}
            />
            <TextInput
                style={styles.input}
                placeholder="Email"
                value={email}
                onChangeText={setEmail}
            />
            <TextInput
                style={styles.input}
                placeholder="Telefon Numarası"
                value={phoneNumber}
                onChangeText={setPhoneNumber}
            />
            <View style={styles.checkboxContainer}>
                <TouchableOpacity
                    onPress={() => setIsPersonel(!isPersonel)}
                    style={[
                        styles.checkbox,
                        isPersonel && styles.checkboxChecked,
                    ]}
                />
                <Text>Personel mi?</Text>
            </View>
            <Button title="Kayıt Ol" onPress={handleRegister} />
            <TouchableOpacity
                style={styles.backButton}
                onPress={() => navigation.goBack()}
            >
                <Text style={styles.backText}>Geri Dön</Text>
            </TouchableOpacity>
        </View>
    );
};

const styles = StyleSheet.create({
    container: {
        flex: 1,
        justifyContent: 'center',
        padding: 16,
    },
    input: {
        height: 40,
        borderColor: 'gray',
        borderWidth: 1,
        marginBottom: 12,
        paddingHorizontal: 8,
    },
    checkboxContainer: {
        flexDirection: 'row',
        alignItems: 'center',
        marginBottom: 16,
    },
    checkbox: {
        width: 20,
        height: 20,
        borderWidth: 1,
        borderColor: 'gray',
        marginRight: 8,
    },
    checkboxChecked: {
        backgroundColor: '#007bff',
    },
    backButton: {
        marginTop: 16,
        alignItems: 'center',
    },
    backText: {
        color: '#007bff',
        fontWeight: 'bold',
    },
});

export default UserRegistrationForm;
