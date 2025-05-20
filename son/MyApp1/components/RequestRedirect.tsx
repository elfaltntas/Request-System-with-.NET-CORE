import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { Text, View, Button, TextInput, StyleSheet } from 'react-native';
import { Picker } from '@react-native-picker/picker';

// Tip tanımlamaları
interface Personel {
    personelId: string;
    firstName: string;
    lastName: string;
}

interface Request {
    requestId: string;
    title: string;
}

interface Unit {
    unitId: string;
    name: string;
}

const RequestRedirect = () => {
    const [sourcePersonnels, setSourcePersonnels] = useState<Personel[]>([]);
    const [sourcePersonelId, setSourcePersonelId] = useState<string>(''); // Personel ID başlangıçta boş
    const [requests, setRequests] = useState<Request[]>([]);
    const [selectedRequestId, setSelectedRequestId] = useState<string>(''); // Request ID başlangıçta boş
    const [targetPersonnels, setTargetPersonnels] = useState<Personel[]>([]);
    const [targetPersonelId, setTargetPersonelId] = useState<string>(''); // Target Personel ID
    const [units, setUnits] = useState<Unit[]>([]);
    const [newUnitId, setNewUnitId] = useState<string>(''); // Yeni Birim ID
    const [notes, setNotes] = useState<string>(''); // Notes
    const [message, setMessage] = useState<string>('');
    const [error, setError] = useState<string>('');
    const [loading, setLoading] = useState<boolean>(false);

    // Veri çekme
    useEffect(() => {
        const fetchData = async () => {
            try {
                setLoading(true);
                const [personelResp, targetResp, unitsResp] = await Promise.all([
                    axios.get('http://10.0.2.2:5121/api/controller/Personel-Listele'),
                    axios.get('http://10.0.2.2:5121/api/controller/Personel-Listele', {
                        params: { onlyActive: true }
                    }),
                    axios.get('http://10.0.2.2:5121/api/controller/GetAllUnits')
                ]);
                setSourcePersonnels(personelResp.data);
                setTargetPersonnels(targetResp.data);
                setUnits(unitsResp.data?.result || unitsResp.data?.units || []);
            } catch (err) {
                setError('Veri alınırken hata oluştu. Lütfen sayfayı yenileyin.');
            } finally {
                setLoading(false);
            }
        };

        fetchData();
    }, []);

    // Personel talep görüntüleme
    useEffect(() => {
        const fetchRequests = async () => {
            if (!sourcePersonelId) {
                setRequests([]);
                setSelectedRequestId('');
                return;
            }

            setLoading(true);
            setError('');
            try {
                const resp = await axios.get('http://10.0.2.2:5121/api/controller/Personel-Talep-Goruntuleme', {
                    params: { personelId: sourcePersonelId }
                });
                setRequests(resp.data);
            } catch (err) {
                setError('Talepler alınırken hata oluştu.');
            } finally {
                setLoading(false);
            }
        };

        fetchRequests();
    }, [sourcePersonelId]);

    // Talep yönlendirme
    const handleRedirect = async () => {
        if (!sourcePersonelId || !selectedRequestId || !newUnitId || !targetPersonelId) {
            setError('Lütfen tüm zorunlu alanları doldurun.');
            setMessage('');
            return;
        }

        setLoading(true);
        setError('');
        setMessage('');
        try {
            const requestData = {
                requestId: selectedRequestId,
                newUnitId: newUnitId,
                targetPersonelId: targetPersonelId,
                newStatus: 1, // Statik değer
                notes: notes
            };

            const response = await axios.post(
                'http://10.0.2.2:5121/api/controller/Talep-Yonlendirme',
                requestData,
                {
                    headers: {
                        'Content-Type': 'application/json',
                    },
                }
            );

            setMessage(response.data?.message || 'Talep başarıyla yönlendirildi.');

            // Formu temizle
            setSourcePersonelId('');
            setSelectedRequestId('');
            setNewUnitId('');
            setTargetPersonelId('');
            setNotes('');
            setRequests([]);
        } catch (err) {
            const errorMessage =
                (err as any)?.response?.data?.message || 'Talep yönlendirilirken hata oluştu.';
            setError(errorMessage);
        }

    };

    return (
        <View style={styles.container}>
            <Text style={styles.header}>Talep Yönlendirme</Text>

            {error && <Text style={styles.error}>{error}</Text>}
            {message && <Text style={styles.success}>{message}</Text>}

            <View style={styles.formGroup}>
                <Text>Talep Sahibi Personel:</Text>
                <Picker
                    selectedValue={sourcePersonelId}
                    onValueChange={(itemValue) => setSourcePersonelId(itemValue)}
                >
                    <Picker.Item label="-- Personel Seçin --" value="" />
                    {sourcePersonnels.map(p => (
                        <Picker.Item key={p.personelId} label={`${p.firstName} ${p.lastName}`} value={p.personelId} />
                    ))}
                </Picker>
            </View>

            <View style={styles.formGroup}>
                <Text>Talep:</Text>
                <Picker
                    selectedValue={selectedRequestId}
                    onValueChange={(itemValue) => setSelectedRequestId(itemValue)}
                >
                    <Picker.Item label="-- Talep Seçin --" value="" />
                    {requests.map(req => (
                        <Picker.Item key={req.requestId} label={req.title || `Talep #${req.requestId}`} value={req.requestId} />
                    ))}
                </Picker>
            </View>

            <View style={styles.formGroup}>
                <Text>Yeni Birim:</Text>
                <Picker
                    selectedValue={newUnitId}
                    onValueChange={(itemValue) => setNewUnitId(itemValue)}
                >
                    <Picker.Item label="-- Birim Seçin --" value="" />
                    {units.map(unit => (
                        <Picker.Item key={unit.unitId} label={unit.name} value={unit.unitId} />
                    ))}
                </Picker>
            </View>

            <View style={styles.formGroup}>
                <Text>Atanacak Personel:</Text>
                <Picker
                    selectedValue={targetPersonelId}
                    onValueChange={(itemValue) => setTargetPersonelId(itemValue)}
                >
                    <Picker.Item label="-- Personel Seçin --" value="" />
                    {targetPersonnels.map(p => (
                        <Picker.Item key={p.personelId} label={`${p.firstName} ${p.lastName}`} value={p.personelId} />
                    ))}
                </Picker>
            </View>

            <View style={styles.formGroup}>
                <Text>Notlar (Opsiyonel):</Text>
                <TextInput
                    value={notes}
                    onChangeText={setNotes}
                    placeholder="Yönlendirme notlarınızı girin"
                    style={styles.textarea}
                />
            </View>

            <Button
                title={loading ? 'Yönlendiriliyor...' : 'Talep Yönlendir'}
                onPress={handleRedirect}
                disabled={loading || !selectedRequestId || !newUnitId || !targetPersonelId}
            />
        </View>
    );
};

const styles = StyleSheet.create({
    container: {
        padding: 20,
        backgroundColor: '#fff',
    },
    header: {
        fontSize: 24,
        fontWeight: 'bold',
        marginBottom: 20,
    },
    formGroup: {
        marginBottom: 20,
    },
    error: {
        color: 'red',
    },
    success: {
        color: 'green',
    },
    textarea: {
        height: 100,
        borderColor: 'gray',
        borderWidth: 1,
        padding: 10,
        marginBottom: 10,
    },
});

export default RequestRedirect;
