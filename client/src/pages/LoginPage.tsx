import { useState } from 'react';
import { Card, Input, Button, Typography, message, Form, theme } from 'antd';
import { UserOutlined, LockOutlined } from '@ant-design/icons';
import { useAuth } from '../contexts/AuthContext';
import { useNavigate } from 'react-router-dom';
import { login } from '../api/auth';

const { Title, Text } = Typography;

export default function LoginPage() {
  const { token: themeToken } = theme.useToken();
  const [userName, setUserName] = useState('');
  const [password, setPassword] = useState('');
  const [loading, setLoading] = useState(false);
  const { loginUser } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async () => {
    if (!userName.trim() || !password.trim()) {
      message.error('Введите имя пользователя и пароль');
      return;
    }
    setLoading(true);

    try {
      const response = await login(userName.trim(), password);
      loginUser(response.user);
      navigate('/');
    } catch (err: unknown) {
      const apiError = err as { response?: { data?: { error?: string } } };
      message.error(apiError?.response?.data?.error || 'Ошибка входа');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div
      style={{
        minHeight: '100vh',
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
        background: themeToken.colorBgLayout,
      }}
    >
      <Card style={{ width: 420, boxShadow: '0 2px 8px rgba(0,0,0,0.09)' }}>
        <Title level={3} style={{ textAlign: 'center' }}>
          MedManage
        </Title>
        <Text type="secondary" style={{ display: 'block', textAlign: 'center', marginBottom: 24 }}>
          Введите имя пользователя и пароль
        </Text>
        <Form onFinish={handleSubmit}>
          <Input
            prefix={<UserOutlined />}
            value={userName}
            onChange={(e) => setUserName(e.target.value)}
            placeholder="Имя пользователя"
            style={{ marginBottom: 16 }}
          />
          <Input.Password
            prefix={<LockOutlined />}
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            placeholder="Пароль"
            style={{ marginBottom: 16 }}
          />
          <Button type="primary" htmlType="submit" loading={loading} block>
            Войти
          </Button>
        </Form>
      </Card>
    </div>
  );
}
