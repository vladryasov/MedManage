import { useState } from 'react';
import { Card, Descriptions, Tag, Button, Typography, Spin, Input, message, Space } from 'antd';
import { EditOutlined, CheckOutlined, CloseOutlined } from '@ant-design/icons';
import { useAuth } from '../contexts/AuthContext';
import { useUpdateUserPhone } from '../hooks/useUsers';
import { userRoleLabels, type UserRole } from '../types';

const { Title } = Typography;

export default function ProfilePage() {
  const { user, loading, logout, updateUser } = useAuth();
  const updatePhoneMutation = useUpdateUserPhone();
  const [editingPhone, setEditingPhone] = useState(false);
  const [phoneValue, setPhoneValue] = useState('');

  const handleEditPhone = () => {
    setPhoneValue(user!.phoneNumber);
    setEditingPhone(true);
  };

  const handleCancelPhone = () => {
    setEditingPhone(false);
  };

  const handleSavePhone = async () => {
    if (!user) return;
    try {
      const updated = { ...user, phoneNumber: phoneValue };
      await updatePhoneMutation.mutateAsync(updated);
      updateUser(updated);
      message.success('Номер телефона обновлён');
      setEditingPhone(false);
    } catch {
      message.error('Ошибка при обновлении номера');
    }
  };

  if (loading || !user) {
    return (
      <div style={{ display: 'flex', justifyContent: 'center', padding: 80 }}>
        <Spin size="large" />
      </div>
    );
  }

  return (
    <Card style={{ maxWidth: 600, margin: '24px auto' }}>
      <Title level={4}>Профиль</Title>
      <Descriptions column={1} bordered size="small">
        <Descriptions.Item label="Имя пользователя">
          {user.userName}
        </Descriptions.Item>
        <Descriptions.Item label="Полное имя">
          {user.fullName}
        </Descriptions.Item>
        <Descriptions.Item label="Роль">
          <Tag color="blue">
            {userRoleLabels[user.role as UserRole] ?? user.role}
          </Tag>
        </Descriptions.Item>
        <Descriptions.Item label="Телефон">
          {editingPhone ? (
            <Space>
              <Input
                value={phoneValue}
                onChange={(e) => setPhoneValue(e.target.value)}
                size="small"
                style={{ width: 180 }}
              />
              <Button
                type="primary"
                size="small"
                icon={<CheckOutlined />}
                onClick={handleSavePhone}
                loading={updatePhoneMutation.isPending}
              />
              <Button
                size="small"
                icon={<CloseOutlined />}
                onClick={handleCancelPhone}
              />
            </Space>
          ) : (
            <Space>
              <span>{user.phoneNumber}</span>
              <Button
                type="text"
                size="small"
                icon={<EditOutlined />}
                onClick={handleEditPhone}
              />
            </Space>
          )}
        </Descriptions.Item>
        <Descriptions.Item label="Создан">
          {new Date(user.createdAt).toLocaleDateString()}
        </Descriptions.Item>
      </Descriptions>
      <Button
        type="primary"
        danger
        onClick={logout}
        style={{ marginTop: 24 }}
      >
        Выйти
      </Button>
    </Card>
  );
}
