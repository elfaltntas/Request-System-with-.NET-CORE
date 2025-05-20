import React, { useState } from 'react';
import {
    View,
    Text,
    TextInput,
    TouchableOpacity,
    StyleSheet,
    Alert,
    ScrollView
} from 'react-native';
import { useForm, Controller } from 'react-hook-form';
import { useNavigation } from '@react-navigation/native';
import { RootStackParamList } from '../App';
import { NativeStackNavigationProp } from '@react-navigation/native-stack';

// Firebase imports
import { initializeApp } from 'firebase/app';
import { getDatabase, ref, set } from 'firebase/database';

// Firebase configuration
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

// Initialize Firebase
const app = initializeApp(firebaseConfig);
const database = getDatabase(app);

type FormData = {
    title: string;
    description: string;
    FirstName: string;
    LastName: string;
};

type RequestFormNavigationProp = NativeStackNavigationProp<RootStackParamList, 'Requests'>;

const RequestForm = () => {
    const {
        control,
        handleSubmit,
        formState: { errors }
    } = useForm<FormData>();
    const [isSubmitting, setIsSubmitting] = useState(false);
    const navigation = useNavigation<RequestFormNavigationProp>();

    const onSubmit = async (data: FormData) => {
        setIsSubmitting(true);
        try {
            // Get a reference to the database path
            const newRequestRef = ref(database, 'Requests/' + Date.now());

            // Push the data to Firebase Realtime Database
            await set(newRequestRef, data);

            Alert.alert(
                'Başarılı',
                `Talep başarıyla oluşturuldu!\nBaşlık: ${data.title}\nAçıklama: ${data.description}\nGönderen: ${data.FirstName} ${data.LastName}`,
                [{ text: 'Tamam', onPress: () => navigation.navigate('Dashboard') }]
            );
        } catch (error) {
            console.error('Talep oluşturma hatası:', error);
            Alert.alert(
                'Hata',
                'Talep oluşturulamadı: ' + (error instanceof Error ? error.message : 'Bilinmeyen hata')
            );
        } finally {
            setIsSubmitting(false);
        }
    };

    return (
        <ScrollView contentContainerStyle={styles.container}>
            <Text style={styles.header}>Yeni Talep Oluştur</Text>

            {/* Başlık */}
            <View style={styles.formGroup}>
                <Text style={styles.label}>Başlık</Text>
                <Controller
                    control={control}
                    rules={{ required: 'Başlık gereklidir' }}
                    render={({ field: { onChange, onBlur, value } }) => (
                        <TextInput
                            style={[styles.input, errors.title && styles.errorInput]}
                            placeholder="Başlık girin"
                            onBlur={onBlur}
                            onChangeText={onChange}
                            value={value}
                        />
                    )}
                    name="title"
                />
                {errors.title && <Text style={styles.errorText}>{errors.title.message}</Text>}
            </View>

            {/* Açıklama */}
            <View style={styles.formGroup}>
                <Text style={styles.label}>Açıklama</Text>
                <Controller
                    control={control}
                    rules={{ required: 'Açıklama gereklidir' }}
                    render={({ field: { onChange, onBlur, value } }) => (
                        <TextInput
                            style={[styles.textArea, errors.description && styles.errorInput]}
                            placeholder="Açıklama girin"
                            multiline
                            numberOfLines={4}
                            onBlur={onBlur}
                            onChangeText={onChange}
                            value={value}
                        />
                    )}
                    name="description"
                />
                {errors.description && <Text style={styles.errorText}>{errors.description.message}</Text>}
            </View>

            {/* Firstname */}
            <View style={styles.formGroup}>
                <Text style={styles.label}>Gönderilen Personel Ad:</Text>
                <Controller
                    control={control}
                    rules={{ required: 'Gönderen adı gereklidir' }}
                    render={({ field: { onChange, onBlur, value } }) => (
                        <TextInput
                            style={[styles.input, errors.FirstName && styles.errorInput]}
                            placeholder="Ad girin"
                            onBlur={onBlur}
                            onChangeText={onChange}
                            value={value}
                        />
                    )}
                    name="FirstName"
                />
                {errors.FirstName && <Text style={styles.errorText}>{errors.FirstName.message}</Text>}
            </View>

            {/* Lastname */}
            <View style={styles.formGroup}>
                <Text style={styles.label}>Gönderilen Personel Soyad:</Text>
                <Controller
                    control={control}
                    rules={{ required: 'Gönderen soyadı gereklidir' }}
                    render={({ field: { onChange, onBlur, value } }) => (
                        <TextInput
                            style={[styles.input, errors.LastName && styles.errorInput]}
                            placeholder="Soyad girin"
                            onBlur={onBlur}
                            onChangeText={onChange}
                            value={value}
                        />
                    )}
                    name="LastName"
                />
                {errors.LastName && <Text style={styles.errorText}>{errors.LastName.message}</Text>}
            </View>

            {/* Submit Button */}
            <TouchableOpacity
                style={styles.submitButton}
                onPress={handleSubmit(onSubmit)}
                disabled={isSubmitting}
            >
                <Text style={styles.submitButtonText}>
                    {isSubmitting ? 'Gönderiliyor...' : 'Talep Oluştur'}
                </Text>
            </TouchableOpacity>
        </ScrollView>
    );
};

const styles = StyleSheet.create({
    container: {
        flexGrow: 1,
        padding: 20,
        backgroundColor: '#f5f5f5',
    },
    header: {
        fontSize: 24,
        fontWeight: 'bold',
        marginBottom: 20,
        textAlign: 'center',
        color: '#333',
    },
    formGroup: {
        marginBottom: 15,
    },
    label: {
        marginBottom: 5,
        fontWeight: '600',
        color: '#333',
    },
    input: {
        backgroundColor: '#fff',
        borderWidth: 1,
        borderColor: '#ddd',
        borderRadius: 5,
        padding: 10,
        fontSize: 16,
    },
    textArea: {
        backgroundColor: '#fff',
        borderWidth: 1,
        borderColor: '#ddd',
        borderRadius: 5,
        padding: 10,
        fontSize: 16,
        height: 100,
        textAlignVertical: 'top',
    },
    errorInput: {
        borderColor: '#ff4444',
    },
    errorText: {
        color: '#ff4444',
        fontSize: 14,
        marginTop: 5,
    },
    submitButton: {
        backgroundColor: '#3498db',
        padding: 15,
        borderRadius: 5,
        alignItems: 'center',
        marginTop: 20,
    },
    submitButtonText: {
        color: '#fff',
        fontWeight: 'bold',
        fontSize: 16,
    },
});

export default RequestForm;
