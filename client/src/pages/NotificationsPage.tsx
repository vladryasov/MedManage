import { Card, Button, Typography, Spin, List, Tag, Space, Empty, message, theme } from 'antd';
import { CheckOutlined } from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';
import { useNotifications, useMarkAsRead, useMarkAllAsRead } from '../hooks/useNotifications';
import {
  inAppNotificationTypeLabels,
  InAppNotificationType,
  type InAppNotificationDTO,
} from '../types';

const { Title } = Typography;

export default function NotificationsPage() {
  const navigate = useNavigate();
  const { data: notifications, isLoading } = useNotifications();
  const markAsReadMutation = useMarkAsRead();
  const markAllAsReadMutation = useMarkAllAsRead();
  const { token } = theme.useToken();

  const handleMarkAsRead = async (id: string) => {
    try {
      await markAsReadMutation.mutateAsync(id);
    } catch {
      message.error('Ошибка при отметке уведомления');
    }
  };

  const handleMarkAllAsRead = async () => {
    try {
      await markAllAsReadMutation.mutateAsync();
      message.success('Все уведомления отмечены прочитанными');
    } catch {
      message.error('Ошибка');
    }
  };

  const getNotificationTab = (type: number) => {
    if (type === InAppNotificationType.PurchaseRequest) return 'incoming';
    return 'history';
  };

  const handleItemClick = (item: InAppNotificationDTO) => {
    if (!item.isRead) {
      markAsReadMutation.mutate(item.id);
    }
    navigate(`/profile?tab=${getNotificationTab(item.type)}`);
  };

  const getTypeTag = (type: number) => {
    const colorMap: Record<number, string> = {
      [InAppNotificationType.PurchaseRequest]: 'blue',
      [InAppNotificationType.RequestAccepted]: 'green',
      [InAppNotificationType.RequestRejected]: 'red',
    };
    return (
      <Tag color={colorMap[type] ?? 'default'}>
        {inAppNotificationTypeLabels[type] ?? type}
      </Tag>
    );
  };

  if (isLoading) {
    return (
      <div style={{ display: 'flex', justifyContent: 'center', padding: 80 }}>
        <Spin size="large" />
      </div>
    );
  }

  return (
    <Card>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 16 }}>
        <Title level={4} style={{ margin: 0 }}>Уведомления</Title>
        {notifications && notifications.some((n) => !n.isRead) && (
          <Button
            type="primary"
            icon={<CheckOutlined />}
            onClick={handleMarkAllAsRead}
            loading={markAllAsReadMutation.isPending}
          >
            Отметить всё прочитанным
          </Button>
        )}
      </div>
      {(!notifications || notifications.length === 0) ? (
        <Empty description="Нет уведомлений" />
      ) : (
        <List
          dataSource={notifications}
          renderItem={(item: InAppNotificationDTO) => (
            <List.Item
              style={{
                background: item.isRead ? 'transparent' : token.colorBgLayout,
                padding: '12px 16px',
                borderRadius: 6,
                marginBottom: 4,
                cursor: 'pointer',
              }}
              onClick={() => handleItemClick(item)}
              actions={
                item.isRead
                  ? []
                  : [
                      <Button
                        type="link"
                        size="small"
                        icon={<CheckOutlined />}
                        onClick={() => handleMarkAsRead(item.id)}
                        loading={markAsReadMutation.isPending}
                      >
                        Прочитано
                      </Button>,
                    ]
              }
            >
              <List.Item.Meta
                title={
                  <Space>
                    {getTypeTag(item.type)}
                    <span style={{ fontWeight: item.isRead ? 'normal' : 'bold' }}>
                      {item.title}
                    </span>
                  </Space>
                }
                description={
                  <div>
                    <div>{item.message}</div>
                    <div style={{ fontSize: 12, color: '#999', marginTop: 4 }}>
                      {new Date(item.createdAt).toLocaleString()}
                    </div>
                  </div>
                }
              />
            </List.Item>
          )}
        />
      )}
    </Card>
  );
}
