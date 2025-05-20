import React, { useState } from 'react';
import {
  View,
  Text,
  TextInput,
  TouchableOpacity,
  StyleSheet,
  ActivityIndicator,
  FlatList,
  Alert,
  ScrollView
} from 'react-native';
import { initializeApp } from 'firebase/app';
import { getDatabase, ref, get } from 'firebase/database';

type Personnel = {
  id: string;
  fullName: string;
  hireDate: string;
  terminationDate: string | null;
  isActive: boolean;
};

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

const PersonnelStatus = () => {
  const [searchFirstName, setSearchFirstName] = useState('');
  const [searchLastName, setSearchLastName] = useState('');
  const [personnelData, setPersonnelData] = useState<Personnel | null>(null);
  const [allPersonnelData, setAllPersonnelData] = useState<Personnel[]>([]);
  const [loading, setLoading] = useState(false);

  // Tek bir personeli isim/soyisim ile bul ve göster
  const handleSearch = async () => {
    if (!searchFirstName.trim() || !searchLastName.trim()) {
      Alert.alert('Uyarı', 'Lütfen hem ad hem soyad girin.');
      return;
    }

    setLoading(true);
    setPersonnelData(null);

    try {
      const snap = await get(ref(db, 'Users'));
      if (!snap.exists()) {
        throw new Error('Veritabanında kullanıcı bulunamadı.');
      }
      const users = snap.val();
      // filtrele
      const matchKey = Object.keys(users).find(key => {
        const u = users[key];
        return (
          u.firstName?.trim().toLowerCase() === searchFirstName.trim().toLowerCase() &&
          u.lastName?.trim().toLowerCase() === searchLastName.trim().toLowerCase()
        );
      });
      if (!matchKey) {
        Alert.alert('Bilgi', 'Aradığınız personel bulunamadı.');
        return;
      }
      const u = users[matchKey];
      setPersonnelData({
        id: matchKey,
        fullName: `${u.firstName} ${u.lastName}`,
        hireDate: u.hireDate,
        terminationDate: u.terminatedAt || null,
        isActive: u.isPersonel ?? false
      });
    } catch (err: any) {
      console.error(err);
      Alert.alert('Hata', err.message);
    } finally {
      setLoading(false);
    }
  };

  // Tüm personelleri al
  const handleFetchAllPersonnel = async () => {
    setLoading(true);
    setAllPersonnelData([]);

    try {
      const snap = await get(ref(db, 'Users'));
      if (!snap.exists()) {
        setAllPersonnelData([]);
        return;
      }
      const users = snap.val();
      const list: Personnel[] = Object.keys(users).map(key => {
        const u = users[key];
        return {
          id: key,
          fullName: `${u.firstName} ${u.lastName}`,
          hireDate: u.hireDate,
          terminationDate: u.terminatedAt || null,
          isActive: u.isPersonel ?? false
        };
      });
      setAllPersonnelData(list);
    } catch (err) {
      console.error(err);
      Alert.alert('Hata', 'Personel listesini alırken sorun oluştu.');
    } finally {
      setLoading(false);
    }
  };

  const renderPersonnelCard = ({ item }: { item: Personnel }) => (
    <View style={styles.card}>
      <Text style={styles.cardText}>Ad Soyad: {item.fullName}</Text>
      <Text style={styles.cardText}>
        İşe Başlama: {new Date(item.hireDate).toLocaleDateString()}
      </Text>
      <Text style={styles.cardText}>
        Çıkış: {item.terminationDate
          ? new Date(item.terminationDate).toLocaleDateString()
          : '—'}
      </Text>
      <Text style={styles.cardText}>
        Durum: {item.isActive ? 'Aktif' : 'Pasif'}
      </Text>
    </View>
  );

  return (
    <ScrollView contentContainerStyle={styles.container}>
      <Text style={styles.header}>Personel Bilgileri</Text>

      <View style={styles.searchContainer}>
        <TextInput
          style={styles.input}
          placeholder="Ad (First Name)"
          value={searchFirstName}
          onChangeText={setSearchFirstName}
        />
        <TextInput
          style={styles.input}
          placeholder="Soyad (Last Name)"
          value={searchLastName}
          onChangeText={setSearchLastName}
        />
        <TouchableOpacity style={styles.button} onPress={handleSearch}>
          <Text style={styles.buttonText}>Ara</Text>
        </TouchableOpacity>
      </View>

      {loading && <ActivityIndicator size="large" color="#0000ff" />}

      {personnelData && (
        <View style={styles.card}>
          <Text style={styles.cardText}>Ad Soyad: {personnelData.fullName}</Text>
          <Text style={styles.cardText}>
            İşe Başlama: {new Date(personnelData.hireDate).toLocaleDateString()}
          </Text>
          <Text style={styles.cardText}>
            Çıkış: {personnelData.terminationDate
              ? new Date(personnelData.terminationDate).toLocaleDateString()
              : '—'}
          </Text>
          <Text style={styles.cardText}>
            Durum: {personnelData.isActive ? 'Aktif' : 'Pasif'}
          </Text>
        </View>
      )}

      <TouchableOpacity style={styles.button} onPress={handleFetchAllPersonnel}>
        <Text style={styles.buttonText}>Tüm Personelleri Listele</Text>
      </TouchableOpacity>

      <FlatList
        data={allPersonnelData}
        renderItem={renderPersonnelCard}
        keyExtractor={(item) => item.id}
        style={styles.list}
      />
    </ScrollView>
  );
};

const styles = StyleSheet.create({
  container: { flexGrow: 1, padding: 16, backgroundColor: '#fff' },
  header: { fontSize: 24, fontWeight: 'bold', textAlign: 'center', marginBottom: 16 },
  searchContainer: { marginBottom: 16 },
  input: {
    borderWidth: 1,
    borderColor: '#ccc',
    borderRadius: 4,
    padding: 8,
    marginBottom: 8,
  },
  button: {
    backgroundColor: '#007bff',
    padding: 10,
    borderRadius: 4,
    alignItems: 'center',
  },
  buttonText: { color: '#fff', fontWeight: 'bold' },
  card: {
    backgroundColor: '#f9f9f9',
    borderRadius: 8,
    padding: 16,
    marginBottom: 16,
  },
  cardText: { fontSize: 16, marginBottom: 4 },
  list: { marginTop: 16 },
});

export default PersonnelStatus;
