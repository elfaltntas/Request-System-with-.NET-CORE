// firebase.ts
import { initializeApp, getApps, getApp } from 'firebase/app';
import { getDatabase } from 'firebase/database';

const firebaseConfig = {
    apiKey: 'AIzaSyBHZ7TzjKyVCmeDH7xXADSDzsfWiFnurMM',
    authDomain: 'requestsystem-6fc37.firebaseapp.com',
    databaseURL: 'https://requestsystem-6fc37-default-rtdb.firebaseio.com',
    projectId: 'requestsystem-6fc37',
    storageBucket: 'requestsystem-6fc37.appspot.com',
    messagingSenderId: '348434724882',
    appId: '1:348434724882:web:306dbc6278cd20d4ca40cb',
};
const app = !getApps().length ? initializeApp(firebaseConfig) : getApp();
export const database = getDatabase(app);

// App.tsx
import React, { useState } from 'react';
import {
    SafeAreaView,
    View,
    Text,
    TextInput,
    TouchableOpacity,
    ActivityIndicator,
    FlatList,
    StyleSheet,
} from 'react-native';
/*import { database } from './firebase';*/
import { ref, get } from 'firebase/database';

interface SubUnit {
    name: string;
    description?: string;
}
interface Unit {
    name: string;
    description?: string;
    subUnits?: SubUnit[];
}

const App: React.FC = () => {
    const [unitName, setUnitName] = useState<string>('');
    const [hierarchy, setHierarchy] = useState<string[]>([]);
    const [loading, setLoading] = useState<boolean>(false);
    const [error, setError] = useState<string>('');

    const handleSearch = async () => {
        setError(''); setHierarchy([]); setLoading(true);
        try {
            // Fetch all units
            const snap = await get(ref(database, 'units'));
            if (!snap.exists()) throw new Error('Ünite verisi bulunamadı');
            const data: Record<string, Unit> = snap.val();
            // Build tree and map name to children
            const map = Object.values(data).map(u => ({
                name: u.name,
                children: u.subUnits?.map(s => s.name) || []
            }));
            // DFS to find path to node and its subtree
            const path: string[] = [];
            const visit = (nodeName: string, ancestors: string[]): boolean => {
                // If matches or child matches
                if (nodeName === unitName) {
                    path.push(...ancestors, nodeName);
                    return true;
                }
                const node = map.find(n => n.name === nodeName);
                if (!node) return false;
                for (let child of node.children) {
                    if (visit(child, [...ancestors, nodeName])) return true;
                }
                return false;
            };
            // Try every root unit
            for (let root of map) {
                if (visit(root.name, [])) break;
            }
            if (path.length === 0) throw new Error('Girdiğiniz ünite bulunamadı');
            // Also append subtree names
            const subtree: string[] = [];
            const collect = (name: string) => {
                subtree.push(name);
                const node = map.find(n => n.name === name);
                node?.children.forEach(collect);
            };
            collect(unitName);
            setHierarchy([...path, ...subtree.filter(n => !path.includes(n))]);
        } catch (e: any) {
            setError(e.message);
        } finally {
            setLoading(false);
        }
    };

    return (
        <SafeAreaView style={styles.container}>
            <Text style={styles.title}>Ünite Hiyerarşisi</Text>
            <TextInput
                style={styles.input}
                placeholder="Ünite adı girin"
                value={unitName}
                onChangeText={setUnitName}
            />
            <TouchableOpacity style={styles.button} onPress={handleSearch} disabled={loading}>
                {loading ? <ActivityIndicator color="#fff" /> : <Text style={styles.buttonText}>Ara</Text>}
            </TouchableOpacity>
            {error ? <Text style={styles.error}>{error}</Text> : null}
            {hierarchy.length > 0 && (
                <FlatList
                    data={hierarchy}
                    keyExtractor={(item, idx) => idx.toString()}
                    renderItem={({ item }) => <Text style={styles.item}>• {item}</Text>}
                />
            )}
        </SafeAreaView>
    );
};

const styles = StyleSheet.create({
    container: { flex: 1, padding: 16 },
    title: { fontSize: 24, fontWeight: 'bold', marginBottom: 16 },
    input: { borderWidth: 1, borderColor: '#ccc', borderRadius: 4, padding: 8, marginBottom: 12 },
    button: { backgroundColor: '#3366ff', padding: 12, borderRadius: 4, alignItems: 'center', marginBottom: 12 },
    buttonText: { color: '#fff', fontWeight: 'bold' },
    error: { color: 'red', marginBottom: 12 },
    item: { fontSize: 16, paddingVertical: 4 }
});

export default App;
