import React, { useState, useEffect } from 'react';
import { Text, StyleSheet, TouchableOpacity, ActivityIndicator } from 'react-native';
import { NavigationContainer } from '@react-navigation/native';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import AsyncStorage from '@react-native-async-storage/async-storage';

// Component imports
import Dashboard from './components/Dashboard';
import LoginForm from './components/LoginForm';
import PersonnelStatus from './components/PersonnelStatus';
import RequestForm from './components/RequestForm';
import UnitList from './components/UnitList';
import RequestApproval from './components/RequestApproval';
import PersonnelAssign from './components/PersonnelAssign';
import RoleManagement from './components/RoleAdd';
import RoleAssign from './components/RoleAssign';
import PersonnelRequestView from './components/PersonnelRequestView';
import RequestStatus from './components/RequestStatus';
import RequestRedirect from './components/RequestRedirect';
import UserRegistrationForm from './components/UserRegistrationForm';
import UnitMove from './components/UnitMove';
import PersonnelList from './components/PersonnelList';
import UnitAdd from './components/UnitAdd';

// Type definitions
export type RootStackParamList = {
    Login: undefined;
    Dashboard: undefined;
    UserRegistration: undefined;
    Personnel: undefined;
    Requests: undefined;
    RequestStatus: undefined;
    Units: undefined;
    RequestApprove: undefined;
    PersonnelAssign: undefined;
    Roles: undefined;
    RoleAssign: undefined;
    PersonnelView: undefined;
    RequestRedirect: undefined;
    UnitMove: undefined;
    PersonnelList: undefined;
};

const Stack = createNativeStackNavigator<RootStackParamList>();

const App: React.FC = () => {
    const [isAuthenticated, setIsAuthenticated] = useState<boolean>(false);
    const [loading, setLoading] = useState<boolean>(true);

    useEffect(() => {
        const checkAuthentication = async () => {
            setLoading(true);
            try {
                const token = await AsyncStorage.getItem('auth_token');
                setIsAuthenticated(!!token);
            } catch (error) {
                console.error('Error checking authentication:', error);
            } finally {
                setLoading(false);
            }
        };

        checkAuthentication();
    }, []);

    const handleLogout = async () => {
        try {
            await AsyncStorage.removeItem('auth_token');
            setIsAuthenticated(false);
        } catch (error) {
            console.error('Error logging out:', error);
        }
    };

    const LogoutButton = () => (
        <TouchableOpacity onPress={handleLogout} style={styles.logoutButton}>
            <Text style={styles.logoutText}>Cikis Yap</Text>
        </TouchableOpacity>
    );

    const renderAuthenticatedScreens = () => (
        <>
            <Stack.Screen
                name="Dashboard"
                component={Dashboard}
                options={{
                    headerRight: () => <LogoutButton />,
                }}
            />
            <Stack.Screen name="Personnel" component={PersonnelStatus} />
            <Stack.Screen name="Requests" component={RequestForm} />
            <Stack.Screen name="RequestStatus" component={RequestStatus} />
            <Stack.Screen name="Units" component={UnitList} />
            <Stack.Screen name="RequestApprove" component={RequestApproval} />
            <Stack.Screen name="PersonnelAssign" component={PersonnelAssign} />
            <Stack.Screen name="Roles" component={RoleManagement} />
            <Stack.Screen name="RoleAssign" component={RoleAssign} />
            <Stack.Screen name="PersonnelView" component={PersonnelRequestView} />
            <Stack.Screen name="RequestRedirect" component={RequestRedirect} />
            <Stack.Screen name="UnitMove" component={UnitMove} />
            <Stack.Screen name="PersonnelList" component={PersonnelList} />
            <Stack.Screen name="UnitAdd" component={UnitAdd} />
        </>
    );

    const renderUnauthenticatedScreens = () => (
        <>
            <Stack.Screen
                name="Login"
                options={{ title: 'Giris Yap' }}
            >
                {(props) => <LoginForm {...props} onLogin={async () => {
                    await AsyncStorage.setItem('auth_token', 'dummy_token');
                    setIsAuthenticated(true);
                }} />}
            </Stack.Screen>
            <Stack.Screen
                name="UserRegistration"
                component={UserRegistrationForm}
                options={{ title: 'Kayit Ol' }}
            />
        </>
    );

    if (loading) {
        return (
            <ActivityIndicator
                style={styles.loading}
                size="large"
                color="#0000ff"
            />
        );
    }

    return (
        <NavigationContainer>
            <Stack.Navigator
                screenOptions={{
                    headerStyle: styles.header,
                    headerTitleStyle: styles.headerTitle,
                }}
            >
                {isAuthenticated ? renderAuthenticatedScreens() : renderUnauthenticatedScreens()}
            </Stack.Navigator>
        </NavigationContainer>
    );
};

const styles = StyleSheet.create({
    header: {
        backgroundColor: '#f8f9fa',
    },
    headerTitle: {
        fontWeight: 'bold',
        color: '#333',
    },
    logoutButton: {
        marginRight: 10,
        padding: 8,
        backgroundColor: '#ff4444',
        borderRadius: 4,
    },
    logoutText: {
        color: 'white',
        fontWeight: 'bold',
    },
    container: {
        flex: 1,
        padding: 16,
        backgroundColor: '#fff',
    },
    loading: {
        flex: 1,
        justifyContent: 'center',
        alignItems: 'center',
    },
});

export default App;
