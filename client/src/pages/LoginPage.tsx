import { useState } from 'react';
import { Card, Input, Button, Typography, message, Form, theme } from 'antd';
import { KeyOutlined } from '@ant-design/icons';
import { useAuth } from '../contexts/AuthContext';
import { useNavigate } from 'react-router-dom';
import { login } from '../api/auth';

const { Title, Text } = Typography;

export default function LoginPage() {
  const { token } = theme.useToken();
  const [jwt, setJwt] = useState('');
  const [loading, setLoading] = useState(false);
  const { loginUser } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async () => {
    if (!jwt.trim()) {
      message.error('Введите JWT токен');
      return;
    }
    setLoading(true);

    try {
      const user = await login(jwt.trim());
      loginUser(jwt.trim(), user);
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
        background: token.colorBgLayout,
      }}
    >
      <Card style={{ width: 420, boxShadow: '0 2px 8px rgba(0,0,0,0.09)' }}>
        <Title level={3} style={{ textAlign: 'center' }}>
          <KeyOutlined /> MedManage
        </Title>
        <Text type="secondary" style={{ display: 'block', textAlign: 'center', marginBottom: 24 }}>
          Введите ваш JWT токен для входа
        </Text>
        <Form onFinish={handleSubmit}>
          <Input.TextArea
            rows={4}
            value={jwt}
            onChange={(e) => setJwt(e.target.value)}
            placeholder="Вставьте JWT токен..."
            style={{ marginBottom: 16, fontFamily: 'monospace', fontSize: 12 }}
          />
          <Button type="primary" htmlType="submit" loading={loading} block>
            Войти
          </Button>
        </Form>
      </Card>
    </div>
  );
}
