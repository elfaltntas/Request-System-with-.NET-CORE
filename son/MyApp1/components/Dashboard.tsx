import React from 'react';
import { View, Text, StyleSheet, TouchableOpacity, ScrollView } from 'react-native';

type DashboardProps = {
    navigation: any;
};

const Dashboard: React.FC<DashboardProps> = ({ navigation }) => {
    const quickActions = [
        { title: 'Talep Oluştur', description: 'Yeni bir talep başlat', route: 'Requests' },
        { title: 'Talep Onayla', description: 'Talep onayı yap', route: 'RequestApprove' },
        { title: 'Personel İşlemleri', description: 'Giriş/Çıkış Yap', route: 'Personnel' },
        { title: 'Rol Ekle', description: 'Yeni bir rol oluştur', route: 'Roles' },
        { title: 'Rol Ata', description: 'Rol atama işlemi', route: 'RoleAssign' },
        { title: 'Personel Talep Görüntüle', description: 'Personel taleplerini görüntüle', route: 'PersonnelView' },
        { title: 'Talep Durum Görüntüle', description: 'Talep durumunu görüntüle', route: 'RequestStatus' },
        { title: 'Birim Taşı', description: 'Birimleri taşımak için tıklayın', route: 'UnitMove' },
        { title: 'İşten Çıkarma', description: 'Personel listesine göz at', route: 'PersonnelList' },
        { title: 'Birimler', description: 'Organizasyon Yapısı', route: 'Units' },
        { title: 'Talep Yönlendir', description: 'Mevcut bir talebi yönlendirin', route: 'RequestRedirect' },
        { title: 'Birim Ekle', description: 'Yeni Birim', route: 'UnitAdd' },
    ];

    const cardColors = [
        '#FF6B6B', '#4D96FF', '#FFD93D', '#6BCB77', '#FF6ECC', '#00D2FF',
        '#FFA500', '#8E2DE2', '#3EB489', '#FF884B', '#FF5C8D', '#7353BA'
    ];

    return (
        <View style={styles.container}>
            <Text style={styles.header}>Yönetim Paneli</Text>
            <ScrollView contentContainerStyle={styles.actionsContainer}>
                {quickActions.map((action, index) => (
                    <TouchableOpacity
                        key={index}
                        style={[
                            styles.actionCard,
                            { backgroundColor: cardColors[index % cardColors.length] },
                        ]}
                        onPress={() => navigation.navigate(action.route)}
                    >
                        <Text style={styles.actionTitle}>{action.title}</Text>
                        <Text style={styles.actionDescription}>{action.description}</Text>
                    </TouchableOpacity>
                ))}
            </ScrollView>
        </View>
    );
};

const styles = StyleSheet.create({
    container: {
        flex: 1,
        padding: 16,
        backgroundColor: '#34495e',
    },
    header: {
        fontSize: 28,
        fontWeight: 'bold',
        textAlign: 'center',
        color: '#34495e',
        marginBottom: 16,
    },
    actionsContainer: {
        flexDirection: 'row',
        flexWrap: 'wrap',
        justifyContent: 'space-between',
        paddingBottom: 16,
    },
    actionCard: {
        width: '47%',
        borderRadius: 12,
        padding: 16,
        marginBottom: 16,
        shadowColor: '#000',
        shadowOffset: { width: 0, height: 4 },
        shadowOpacity: 0.1,
        shadowRadius: 4,
        elevation: 4,
    },
    actionTitle: {
        fontSize: 16,
        fontWeight: 'bold',
        color: '#fff',
        marginBottom: 8,
    },
    actionDescription: {
        fontSize: 14,
        color: '#ecf0f1',
    },
});

export default Dashboard;
