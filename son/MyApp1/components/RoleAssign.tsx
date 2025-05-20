import React, { useState, useEffect } from 'react';
import {
    View,
    Text,
    StyleSheet,
    TouchableOpacity,
    FlatList,
    Modal,
    ActivityIndicator,
    Alert,
} from 'react-native';
import { getDatabase, ref, get, set, push } from 'firebase/database';
import { initializeApp, getApps, getApp } from 'firebase/app';

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
const app = !getApps().length ? initializeApp(firebaseConfig) : getApp();
const database = getDatabase(app);

interface User {
    userId: string;
    firstName: string;
    lastName: string;
    email: string;
    roleId?: string;
    unitId?: string;
}

interface Role {
    roleId: string;
    name: string;
}

interface Unit {
    unitId: string;
    name: string;
}

interface PersonnelRole {
    assignmentId?: string;
    userId: string;
    roleId: string;
    assignedAt: number;
}

interface PersonnelUnit {
    assignmentId?: string;
    userId: string;
    unitId: string;
    assignedAt: number;
}

const RoleAssign: React.FC = () => {
    const [selectedUser, setSelectedUser] = useState<User | null>(null);
    const [selectedRole, setSelectedRole] = useState<Role | null>(null);
    const [selectedUnit, setSelectedUnit] = useState<Unit | null>(null);
    const [users, setUsers] = useState<User[]>([]);
    const [roles, setRoles] = useState<Role[]>([]);
    const [units, setUnits] = useState<Unit[]>([]);
    const [personnelRoles, setPersonnelRoles] = useState<PersonnelRole[]>([]);
    const [personnelUnits, setPersonnelUnits] = useState<PersonnelUnit[]>([]);
    const [loading, setLoading] = useState(false);
    const [isUserModalVisible, setUserModalVisible] = useState(false);
    const [isRoleModalVisible, setRoleModalVisible] = useState(false);
    const [isUnitModalVisible, setUnitModalVisible] = useState(false);

    useEffect(() => {
        const fetchData = async () => {
            try {
                setLoading(true);

                // Fetch Users
                const usersRef = ref(database, 'Users');
                const usersSnapshot = await get(usersRef);
                if (usersSnapshot.exists()) {
                    const usersData = usersSnapshot.val();
                    const usersArray = Object.keys(usersData).map(key => ({
                        userId: key,
                        ...usersData[key]
                    }));
                    setUsers(usersArray);
                }

                // Fetch Roles
                const rolesRef = ref(database, 'Roles');
                const rolesSnapshot = await get(rolesRef);
                if (rolesSnapshot.exists()) {
                    const rolesData = rolesSnapshot.val();
                    const rolesArray = Object.keys(rolesData).map(key => ({
                        roleId: key,
                        ...rolesData[key]
                    }));
                    setRoles(rolesArray);
                }

                // Fetch Units
                const unitsRef = ref(database, 'units');
                const unitsSnapshot = await get(unitsRef);
                if (unitsSnapshot.exists()) {
                    const unitsData = unitsSnapshot.val();
                    const unitsArray = Object.keys(unitsData).map(key => ({
                        unitId: key,
                        ...unitsData[key]
                    }));
                    setUnits(unitsArray);
                }

                // Fetch Personnel Roles
                const personnelRolesRef = ref(database, 'PersonelRoles');
                const personnelRolesSnapshot = await get(personnelRolesRef);
                if (personnelRolesSnapshot.exists()) {
                    const personnelRolesData = personnelRolesSnapshot.val();
                    const personnelRolesArray = Object.keys(personnelRolesData).map(key => ({
                        assignmentId: key,
                        ...personnelRolesData[key]
                    }));
                    setPersonnelRoles(personnelRolesArray);
                }

                // Fetch Personnel Units
                const personnelUnitsRef = ref(database, 'PersonelUnits');
                const personnelUnitsSnapshot = await get(personnelUnitsRef);
                if (personnelUnitsSnapshot.exists()) {
                    const personnelUnitsData = personnelUnitsSnapshot.val();
                    const personnelUnitsArray = Object.keys(personnelUnitsData).map(key => ({
                        assignmentId: key,
                        ...personnelUnitsData[key]
                    }));
                    setPersonnelUnits(personnelUnitsArray);
                }
            } catch (error) {
                Alert.alert('Error', 'There was an issue fetching the data.');
                console.error('Firebase error:', error);
            } finally {
                setLoading(false);
            }
        };

        fetchData();
    }, []);

    const handleRoleAssign = async () => {
        if (!selectedUser || !selectedRole) {
            Alert.alert('Warning', 'Please select both a user and a role.');
            return;
        }

        setLoading(true);

        try {
            // Add new role assignment
            const newAssignmentRef = push(ref(database, 'PersonelRoles'));
            const newAssignment: PersonnelRole = {
                userId: selectedUser.userId,
                roleId: selectedRole.roleId,
                assignedAt: Date.now()
            };

            await set(newAssignmentRef, newAssignment);

            // Update user's role in Users table
            const userRef = ref(database, `Users/${selectedUser.userId}/roleId`);
            await set(userRef, selectedRole.roleId);

            Alert.alert('Success', 'Role assigned successfully!');
            setSelectedRole(null);

            // Update local state
            setPersonnelRoles(prev => [...prev, {
                assignmentId: newAssignmentRef.key,
                ...newAssignment
            }]);

            // Update user in local state
            setUsers(prev => prev.map(user =>
                user.userId === selectedUser.userId
                    ? { ...user, roleId: selectedRole.roleId }
                    : user
            ));
        } catch (error) {
            Alert.alert('Error', 'Role assignment failed.');
            console.error('Role assignment error:', error);
        } finally {
            setLoading(false);
        }
    };

    const handleUnitAssign = async () => {
        if (!selectedUser || !selectedUnit) {
            Alert.alert('Warning', 'Please select both a user and a unit.');
            return;
        }

        setLoading(true);

        try {
            // Add new unit assignment
            const newAssignmentRef = push(ref(database, 'PersonelUnits'));
            const newAssignment: PersonnelUnit = {
                userId: selectedUser.userId,
                unitId: selectedUnit.unitId,
                assignedAt: Date.now()
            };

            await set(newAssignmentRef, newAssignment);

            // Update user's unit in Users table
            const userRef = ref(database, `Users/${selectedUser.userId}/unitId`);
            await set(userRef, selectedUnit.unitId);

            Alert.alert('Success', 'Unit assigned successfully!');
            setSelectedUnit(null);

            // Update local state
            setPersonnelUnits(prev => [...prev, {
                assignmentId: newAssignmentRef.key,
                ...newAssignment
            }]);

            // Update user in local state
            setUsers(prev => prev.map(user =>
                user.userId === selectedUser.userId
                    ? { ...user, unitId: selectedUnit.unitId }
                    : user
            ));
        } catch (error) {
            Alert.alert('Error', 'Unit assignment failed.');
            console.error('Unit assignment error:', error);
        } finally {
            setLoading(false);
        }
    };

    const getUserCurrentRole = (userId: string) => {
        const roleAssignment = personnelRoles.find(pr => pr.userId === userId);
        if (!roleAssignment) return 'No role assigned';

        const role = roles.find(r => r.roleId === roleAssignment.roleId);
        return role ? role.name : 'Unknown role';
    };

    const getUserCurrentUnit = (userId: string) => {
        const unitAssignment = personnelUnits.find(pu => pu.userId === userId);
        if (!unitAssignment) return 'No unit assigned';

        const unit = units.find(u => u.unitId === unitAssignment.unitId);
        return unit ? unit.name : 'Unknown unit';
    };

    const renderItem = (item: any, type: 'user' | 'role' | 'unit') => (
        <TouchableOpacity
            style={styles.modalItem}
            onPress={() => {
                if (type === 'user') {
                    setSelectedUser(item);
                    setUserModalVisible(false);
                } else if (type === 'role') {
                    setSelectedRole(item);
                    setRoleModalVisible(false);
                } else {
                    setSelectedUnit(item);
                    setUnitModalVisible(false);
                }
            }}
        >
            <View>
                <Text style={styles.itemTitle}>
                    {type === 'user' ? `${item.firstName} ${item.lastName}` :
                        type === 'role' ? item.name : item.name}
                </Text>
                {type === 'user' && (
                    <>
                        <Text style={styles.itemSubtitle}>
                            Current Role: {getUserCurrentRole(item.userId)}
                        </Text>
                        <Text style={styles.itemSubtitle}>
                            Current Unit: {getUserCurrentUnit(item.userId)}
                        </Text>
                    </>
                )}
            </View>
        </TouchableOpacity>
    );

    return (
        <View style={styles.container}>
            <Text style={styles.header}>Role and Unit Assignment</Text>

            {loading && <ActivityIndicator size="large" style={styles.loader} />}

            <Text style={styles.label}>Select User:</Text>
            <TouchableOpacity
                style={styles.dropdown}
                onPress={() => setUserModalVisible(true)}
                disabled={loading}
            >
                <Text>
                    {selectedUser
                        ? `${selectedUser.firstName} ${selectedUser.lastName}`
                        : 'Select User'}
                </Text>
            </TouchableOpacity>

            <Text style={styles.label}>Select Role:</Text>
            <TouchableOpacity
                style={styles.dropdown}
                onPress={() => setRoleModalVisible(true)}
                disabled={loading || !selectedUser}
            >
                <Text>{selectedRole ? selectedRole.name : 'Select Role'}</Text>
            </TouchableOpacity>

            <TouchableOpacity
                style={[styles.button, (loading || !selectedUser || !selectedRole) && styles.disabledButton]}
                onPress={handleRoleAssign}
                disabled={loading || !selectedUser || !selectedRole}
            >
                <Text style={styles.buttonText}>Assign Role</Text>
            </TouchableOpacity>

            <Text style={styles.label}>Select Unit:</Text>
            <TouchableOpacity
                style={styles.dropdown}
                onPress={() => setUnitModalVisible(true)}
                disabled={loading || !selectedUser}
            >
                <Text>{selectedUnit ? selectedUnit.name : 'Select Unit'}</Text>
            </TouchableOpacity>

            <TouchableOpacity
                style={[styles.button, (loading || !selectedUser || !selectedUnit) && styles.disabledButton]}
                onPress={handleUnitAssign}
                disabled={loading || !selectedUser || !selectedUnit}
            >
                <Text style={styles.buttonText}>Assign Unit</Text>
            </TouchableOpacity>

            {/* User Selection Modal */}
            <Modal
                visible={isUserModalVisible}
                animationType="slide"
                onRequestClose={() => setUserModalVisible(false)}
            >
                <View style={styles.modalContainer}>
                    <Text style={styles.modalTitle}>Select User</Text>
                    <FlatList
                        data={users}
                        keyExtractor={(item) => item.userId}
                        renderItem={({ item }) => renderItem(item, 'user')}
                    />
                    <TouchableOpacity
                        style={styles.closeButton}
                        onPress={() => setUserModalVisible(false)}
                    >
                        <Text style={styles.buttonText}>Close</Text>
                    </TouchableOpacity>
                </View>
            </Modal>

            {/* Role Selection Modal */}
            <Modal
                visible={isRoleModalVisible}
                animationType="slide"
                onRequestClose={() => setRoleModalVisible(false)}
            >
                <View style={styles.modalContainer}>
                    <Text style={styles.modalTitle}>Select Role</Text>
                    <FlatList
                        data={roles}
                        keyExtractor={(item) => item.roleId}
                        renderItem={({ item }) => renderItem(item, 'role')}
                    />
                    <TouchableOpacity
                        style={styles.closeButton}
                        onPress={() => setRoleModalVisible(false)}
                    >
                        <Text style={styles.buttonText}>Close</Text>
                    </TouchableOpacity>
                </View>
            </Modal>

            {/* Unit Selection Modal */}
            <Modal
                visible={isUnitModalVisible}
                animationType="slide"
                onRequestClose={() => setUnitModalVisible(false)}
            >
                <View style={styles.modalContainer}>
                    <Text style={styles.modalTitle}>Select Unit</Text>
                    <FlatList
                        data={units}
                        keyExtractor={(item) => item.unitId}
                        renderItem={({ item }) => renderItem(item, 'unit')}
                    />
                    <TouchableOpacity
                        style={styles.closeButton}
                        onPress={() => setUnitModalVisible(false)}
                    >
                        <Text style={styles.buttonText}>Close</Text>
                    </TouchableOpacity>
                </View>
            </Modal>
        </View>
    );
};

const styles = StyleSheet.create({
    container: {
        flex: 1,
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
    label: {
        fontSize: 16,
        marginBottom: 8,
        color: '#555',
    },
    dropdown: {
        borderWidth: 1,
        borderColor: '#ccc',
        borderRadius: 8,
        padding: 12,
        marginBottom: 16,
        backgroundColor: '#fff',
    },
    button: {
        backgroundColor: '#007BFF',
        padding: 12,
        borderRadius: 8,
        alignItems: 'center',
        marginVertical: 10,
    },
    buttonText: {
        color: '#fff',
        fontSize: 16,
        fontWeight: 'bold',
    },
    disabledButton: {
        backgroundColor: '#aaa',
    },
    modalContainer: {
        flex: 1,
        padding: 20,
        backgroundColor: '#fff',
    },
    modalTitle: {
        fontSize: 20,
        fontWeight: 'bold',
        marginBottom: 20,
        textAlign: 'center',
        color: '#333',
    },
    modalItem: {
        padding: 12,
        borderBottomWidth: 1,
        borderBottomColor: '#eee',
    },
    itemTitle: {
        fontSize: 16,
        color: '#333',
    },
    itemSubtitle: {
        fontSize: 14,
        color: '#777',
        marginTop: 4,
    },
    closeButton: {
        backgroundColor: '#FF6347',
        padding: 12,
        borderRadius: 8,
        alignItems: 'center',
        marginTop: 20,
    },
    loader: {
        marginVertical: 20,
    },
});

export default RoleAssign;