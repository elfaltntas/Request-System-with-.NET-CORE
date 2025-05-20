import React, { useState } from 'react';
import { View, Text, TextInput, Button, StyleSheet, Alert, TouchableOpacity } from 'react-native';
import { useNavigation } from '@react-navigation/native';
import { NativeStackNavigationProp } from '@react-navigation/native-stack';
import { RootStackParamList } from '../App'; // Veya doğrudan tanımlayın
//import auth from '@react-native-firebase/auth';
//import firestore from '@react-native-firebase/firestore';


type AppNavigationProp = NativeStackNavigationProp<RootStackParamList, 'Login'>;

interface LoginFormProps {
    onLogin: () => void;
}

const LoginForm: React.FC<LoginFormProps> = ({ onLogin }) => {
    const [username, setUsername] = useState<string>('');
    const [password, setPassword] = useState<string>('');
    const [showPassword, setShowPassword] = useState<boolean>(false);
    const navigation = useNavigation<AppNavigationProp>();

    const handleLogin = async () => {
        try {
            const response = await fetch('http://10.0.2.2:5121/api/controller/login', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ username, password }),
            });

            const responseText = await response.text();
            const data = responseText ? JSON.parse(responseText) : null;

            if (!response.ok) throw new Error(data?.message || 'Giriş başarısız');

            Alert.alert('Başarılı', `Hoşgeldiniz`);
            onLogin(); // App.tsx'teki isAuthenticated'ı true yapar
        } catch (error) {
            Alert.alert('Hata', error instanceof Error ? error.message : 'Bilinmeyen hata');
        }
    };

    return (
        <View style={styles.container}>
            <Text style={styles.header}>Giriş Yap</Text>
            <TextInput
                style={styles.input}
                placeholder="Kullanıcı Adı"
                value={username}
                onChangeText={setUsername}
            />
            <View style={styles.passwordContainer}>
                <TextInput
                    style={styles.passwordInput}
                    placeholder="Şifre"
                    value={password}
                    onChangeText={setPassword}
                    secureTextEntry={!showPassword}
                />
                <TouchableOpacity
                    onPress={() => setShowPassword((prev) => !prev)}
                    style={styles.eyeIcon}
                >
                    <Text>{showPassword ? '🙈' : '👁️'}</Text>
                </TouchableOpacity>
            </View>
            <View style={styles.buttonContainer}>
                <Button title="Giriş Yap" onPress={handleLogin} />
            </View>
            <View style={styles.buttonContainer}>
                <Button
                    title="Kayıt Ol"
                    onPress={() => navigation.navigate('UserRegistration')}
                />
            </View>
        </View>
    );
};

const styles = StyleSheet.create({
    container: {
        flex: 1,
        justifyContent: 'center',
        padding: 20,
        backgroundColor: '#f5f5f5',
    },
    header: {
        fontSize: 24,
        fontWeight: 'bold',
        marginBottom: 20,
        textAlign: 'center',
    },
    input: {
        height: 50,
        borderColor: '#ddd',
        borderWidth: 1,
        borderRadius: 8,
        marginBottom: 15,
        paddingHorizontal: 10,
        backgroundColor: '#fff',
    },
    passwordContainer: {
        flexDirection: 'row',
        alignItems: 'center',
        borderColor: '#ddd',
        borderWidth: 1,
        borderRadius: 8,
        marginBottom: 15,
        backgroundColor: '#fff',
    },
    passwordInput: {
        flex: 1,
        height: 50,
        paddingHorizontal: 10,
    },
    eyeIcon: {
        padding: 10,
    },
    buttonContainer: {
        marginVertical: 10,
    },
});

export default LoginForm;
