import { useState, useEffect } from 'react';
import { useParams, useNavigate, useLocation } from 'react-router-dom';
import { useQueryClient } from '@tanstack/react-query';
import {
  Card,
  Descriptions,
  Tag,
  Button,
  Input,
  Typography,
  message,
  Spin,
  Space,
  Modal,
} from 'antd';
import { ArrowLeftOutlined, EditOutlined, ShoppingCartOutlined } from '@ant-design/icons';
import { useAnnouncement } from '../hooks/useAnnouncements';
import { useCreatePurchaseRequest } from '../hooks/usePurchaseRequests';
import { updateAnnouncementContent } from '../api/announcements';
import {
  inventoryStatusLabels,
  productTypeLabels,
  InventoryStatus,
  ProductType,
} from '../types';
import { useAuth } from '../contexts/AuthContext';
import dayjs from 'dayjs';

const { Title, Paragraph } = Typography;

const statusColors: Record<number, string> = {
  [InventoryStatus.All]: 'default',
  [InventoryStatus.InStock]: 'green',
  [InventoryStatus.OutOfStock]: 'red',
  [InventoryStatus.Replenishing]: 'blue',
  [InventoryStatus.Expired]: 'orange',
};

export default function AnnouncementDetailPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const { data: announcement, isLoading } = useAnnouncement(id!);
  const location = useLocation();
  const { user } = useAuth();
  const createRequestMutation = useCreatePurchaseRequest();

  const [editing, setEditing] = useState(location.state?.startEditing === true);
  const [newContent, setNewContent] = useState('');
  const [requestModalOpen, setRequestModalOpen] = useState(false);
  const [requestMessage, setRequestMessage] = useState('');

  useEffect(() => {
    if (editing && announcement) {
      setNewContent(announcement.content);
    }
  }, [editing, announcement]);

  const handleSaveContent = async () => {
    try {
      await updateAnnouncementContent(id!, newContent);
      message.success('Содержание обновлено');
      queryClient.invalidateQueries({ queryKey: ['announcement', id] });
      setEditing(false);
    } catch {
      message.error('Ошибка при обновлении');
    }
  };

  const handleRequestPurchase = async () => {
    if (!id) return;
    try {
      await createRequestMutation.mutateAsync({
        announcementId: id,
        message: requestMessage || undefined,
      });
      message.success('Запрос на покупку отправлен');
      setRequestModalOpen(false);
      setRequestMessage('');
    } catch {
      message.error('Ошибка при отправке запроса');
    }
  };

  const isOwnAnnouncement = user && announcement && user.userId === announcement.createdByUserId;
  const canRequest = announcement?.statusInventory === InventoryStatus.InStock && !isOwnAnnouncement;

  if (isLoading) {
    return (
      <div style={{ display: 'flex', justifyContent: 'center', padding: 80 }}>
        <Spin size="large" />
      </div>
    );
  }

  if (!announcement) {
    return <Card>Объявление не найдено</Card>;
  }

  return (
    <Card>
      <Button
        type="text"
        icon={<ArrowLeftOutlined />}
        onClick={() => navigate('/')}
        style={{ marginBottom: 16 }}
      >
        Назад
      </Button>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <Title level={4} style={{ margin: 0 }}>{announcement.title}</Title>
        {canRequest && (
          <Button
            type="primary"
            icon={<ShoppingCartOutlined />}
            onClick={() => setRequestModalOpen(true)}
            loading={createRequestMutation.isPending}
          >
            Запросить покупку
          </Button>
        )}
      </div>
      <Descriptions column={1} bordered size="small" style={{ marginTop: 16, marginBottom: 24 }}>
        <Descriptions.Item label="Автор">{announcement.userName}</Descriptions.Item>
        <Descriptions.Item label="Тип">
          {productTypeLabels[announcement.typeProduct as ProductType] ??
            announcement.typeProduct}
        </Descriptions.Item>
        <Descriptions.Item label="Статус">
          <Tag
            color={
              statusColors[announcement.statusInventory as InventoryStatus] ??
              'default'
            }
          >
            {inventoryStatusLabels[announcement.statusInventory as InventoryStatus] ??
              announcement.statusInventory}
          </Tag>
        </Descriptions.Item>
        <Descriptions.Item label="Просмотры">{announcement.views}</Descriptions.Item>
        <Descriptions.Item label="Создано">
          {dayjs(announcement.createdAt).format('DD.MM.YYYY HH:mm')}
        </Descriptions.Item>
        {announcement.expirationDate && (
          <Descriptions.Item label="Истекает">
            {dayjs(announcement.expirationDate).format('DD.MM.YYYY')}
          </Descriptions.Item>
        )}
        {announcement.updatedAt && (
          <Descriptions.Item label="Обновлено">
            {dayjs(announcement.updatedAt).format('DD.MM.YYYY HH:mm')}
          </Descriptions.Item>
        )}
      </Descriptions>

      <div style={{ marginBottom: 8 }}>
        <strong>Содержание</strong>
        {!editing && (
          <Button
            type="link"
            icon={<EditOutlined />}
            onClick={() => {
              setNewContent(announcement.content);
              setEditing(true);
            }}
          >
            Редактировать
          </Button>
        )}
      </div>
      {editing ? (
        <div>
          <Input.TextArea
            rows={6}
            value={newContent}
            onChange={(e) => setNewContent(e.target.value)}
            autoFocus
          />
          <Space style={{ marginTop: 8 }}>
            <Button type="primary" onClick={handleSaveContent}>
              Сохранить
            </Button>
            <Button onClick={() => setEditing(false)}>Отмена</Button>
          </Space>
        </div>
      ) : (
        <Paragraph>{announcement.content}</Paragraph>
      )}

      <Modal
        title="Запрос на покупку"
        open={requestModalOpen}
        onOk={handleRequestPurchase}
        onCancel={() => {
          setRequestModalOpen(false);
          setRequestMessage('');
        }}
        confirmLoading={createRequestMutation.isPending}
        okText="Отправить"
        cancelText="Отмена"
      >
        <p>Вы хотите купить: <strong>{announcement.title}</strong></p>
        <Input.TextArea
          rows={3}
          value={requestMessage}
          onChange={(e) => setRequestMessage(e.target.value)}
          placeholder="Сообщение продавцу (необязательно)"
        />
      </Modal>
    </Card>
  );
}
