import { useState } from 'react';
import {
  Card,
  Descriptions,
  Tag,
  Button,
  Typography,
  Spin,
  Input,
  message,
  Space,
  Tabs,
  Table,
  Popconfirm,
} from 'antd';
import { EditOutlined, CheckOutlined, CloseOutlined, CheckCircleOutlined, CloseCircleOutlined, DeleteOutlined } from '@ant-design/icons';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { useAuth } from '../contexts/AuthContext';
import { useUpdateUserPhone } from '../hooks/useUsers';
import { useMyAnnouncements } from '../hooks/useAnnouncements';
import { deleteAnnouncement } from '../api/announcements';
import {
  useIncomingRequests,
  useOutgoingRequests,
  useAcceptPurchaseRequest,
  useRejectPurchaseRequest,
  useDeletePurchaseRequest,
} from '../hooks/usePurchaseRequests';
import {
  userRoleLabels,
  inventoryStatusLabels,
  productTypeLabels,
  purchaseRequestStatusLabels,
  type UserRole,
  type InventoryStatus,
  type ProductType,
  type PurchaseRequestDTO,
} from '../types';

const { Title } = Typography;

const statusColors: Record<number, string> = {
  [0]: 'default',
  [1]: 'green',
  [2]: 'red',
};

export default function ProfilePage() {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const [searchParams, setSearchParams] = useSearchParams();
  const { user, loading, logout, updateUser } = useAuth();
  const [activeTab, setActiveTab] = useState(searchParams.get('tab') || 'profile');

  const { data: myAnnouncements, isLoading: announcementsLoading } = useMyAnnouncements();
  const { data: incomingRequests, isLoading: incomingLoading } = useIncomingRequests();
  const { data: outgoingRequests, isLoading: outgoingLoading } = useOutgoingRequests();

  const acceptMutation = useAcceptPurchaseRequest();
  const rejectMutation = useRejectPurchaseRequest();
  const deleteMutation = useDeletePurchaseRequest();
  const deleteAnnouncementMutation = useMutation({
    mutationFn: deleteAnnouncement,
    onSuccess: () => {
      message.success('Объявление удалено');
      queryClient.invalidateQueries({ queryKey: ['announcements'] });
    },
  });

  const pendingIncoming = (incomingRequests ?? []).filter((r) => r.status === 0);
  const pendingOutgoing = (outgoingRequests ?? []).filter((r) => r.status === 0);
  const historyRequests = [
    ...((incomingRequests ?? []).filter((r) => r.status !== 0).map((r) => ({ ...r, _direction: 'Входящий' as const }))),
    ...((outgoingRequests ?? []).filter((r) => r.status !== 0).map((r) => ({ ...r, _direction: 'Исходящий' as const }))),
  ].sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());

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

  const handleAccept = async (id: string) => {
    try {
      await acceptMutation.mutateAsync(id);
      message.success('Запрос принят, объявление удалено');
    } catch {
      message.error('Ошибка при принятии запроса');
    }
  };

  const handleReject = async (id: string) => {
    try {
      await rejectMutation.mutateAsync(id);
      message.success('Запрос отклонён');
    } catch {
      message.error('Ошибка при отклонении запроса');
    }
  };

  const handleDelete = async (id: string) => {
    try {
      await deleteMutation.mutateAsync(id);
      message.success('Запрос удалён');
    } catch {
      message.error('Ошибка при удалении запроса');
    }
  };

  const handleTabChange = (key: string) => {
    setActiveTab(key);
    const params = new URLSearchParams(searchParams);
    if (key === 'profile') {
      params.delete('tab');
    } else {
      params.set('tab', key);
    }
    setSearchParams(params, { replace: true });
  };

  if (loading || !user) {
    return (
      <div style={{ display: 'flex', justifyContent: 'center', padding: 80 }}>
        <Spin size="large" />
      </div>
    );
  }

  const incomingColumns = [
    {
      title: 'Товар',
      dataIndex: 'announcementTitle',
      key: 'announcementTitle',
      render: (val: string | null) => val ?? '—',
    },
    {
      title: 'Покупатель',
      dataIndex: 'buyerUserName',
      key: 'buyerUserName',
    },
    {
      title: 'Сообщение',
      dataIndex: 'message',
      key: 'message',
      render: (val: string | null) => val ?? '—',
      ellipsis: true,
    },
    {
      title: 'Статус',
      dataIndex: 'status',
      key: 'status',
      render: (val: number) => (
        <Tag color={statusColors[val] || 'default'}>{purchaseRequestStatusLabels[val] ?? val}</Tag>
      ),
    },
    {
      title: 'Дата',
      dataIndex: 'createdAt',
      key: 'createdAt',
      render: (val: string) => new Date(val).toLocaleDateString(),
    },
    {
      title: 'Действия',
      key: 'actions',
      render: (_: unknown, record: PurchaseRequestDTO) => (
        <Space>
          {record.status === 0 && (
            <>
              <Popconfirm title="Принять запрос?" onConfirm={() => handleAccept(record.purchaseRequestId)}>
                <Button
                  type="primary"
                  size="small"
                  icon={<CheckCircleOutlined />}
                  loading={acceptMutation.isPending}
                >
                  Принять
                </Button>
              </Popconfirm>
              <Popconfirm title="Отклонить запрос?" onConfirm={() => handleReject(record.purchaseRequestId)}>
                <Button
                  size="small"
                  icon={<CloseCircleOutlined />}
                  loading={rejectMutation.isPending}
                >
                  Отклонить
                </Button>
              </Popconfirm>
            </>
          )}
        </Space>
      ),
    },
  ];

  const outgoingColumns = [
    {
      title: 'Товар',
      dataIndex: 'announcementTitle',
      key: 'announcementTitle',
      render: (val: string | null) => val ?? '—',
    },
    {
      title: 'Продавец',
      dataIndex: 'sellerUserName',
      key: 'sellerUserName',
    },
    {
      title: 'Сообщение',
      dataIndex: 'message',
      key: 'message',
      render: (val: string | null) => val ?? '—',
      ellipsis: true,
    },
    {
      title: 'Статус',
      dataIndex: 'status',
      key: 'status',
      render: (val: number) => (
        <Tag color={statusColors[val] || 'default'}>{purchaseRequestStatusLabels[val] ?? val}</Tag>
      ),
    },
    {
      title: 'Дата',
      dataIndex: 'createdAt',
      key: 'createdAt',
      render: (val: string) => new Date(val).toLocaleDateString(),
    },
    {
      title: 'Действия',
      key: 'actions',
      render: (_: unknown, record: PurchaseRequestDTO) => (
        record.status === 0 && (
          <Popconfirm title="Удалить запрос?" onConfirm={() => handleDelete(record.purchaseRequestId)}>
            <Button
              size="small"
              danger
              icon={<DeleteOutlined />}
              loading={deleteMutation.isPending}
            />
          </Popconfirm>
        )
      ),
    },
  ];

  const historyColumns = [
    {
      title: 'Товар',
      dataIndex: 'announcementTitle',
      key: 'announcementTitle',
      render: (val: string | null) => val ?? '—',
    },
    {
      title: 'Контрагент',
      key: 'counterparty',
      render: (_: unknown, record: any) => record._direction === 'Входящий' ? record.buyerUserName : record.sellerUserName,
    },
    {
      title: 'Тип',
      dataIndex: '_direction',
      key: 'direction',
      render: (val: string) => (
        <Tag color={val === 'Входящий' ? 'blue' : 'orange'}>{val}</Tag>
      ),
    },
    {
      title: 'Статус',
      dataIndex: 'status',
      key: 'status',
      render: (val: number) => (
        <Tag color={statusColors[val] || 'default'}>{purchaseRequestStatusLabels[val] ?? val}</Tag>
      ),
    },
    {
      title: 'Дата',
      dataIndex: 'createdAt',
      key: 'createdAt',
      render: (val: string) => new Date(val).toLocaleDateString(),
    },
  ];

  const announcementColumns = [
    {
      title: 'Заголовок',
      dataIndex: 'title',
      key: 'title',
    },
    {
      title: 'Тип',
      dataIndex: 'typeProduct',
      key: 'typeProduct',
      render: (val: number) => productTypeLabels[val as ProductType] ?? val,
    },
    {
      title: 'Статус',
      dataIndex: 'statusInventory',
      key: 'statusInventory',
      render: (val: number) => (
        <Tag color={statusColors[val] || 'default'}>{inventoryStatusLabels[val as InventoryStatus] ?? val}</Tag>
      ),
    },
    {
      title: 'Просмотры',
      dataIndex: 'views',
      key: 'views',
    },
    {
      title: 'Создано',
      dataIndex: 'createdAt',
      key: 'createdAt',
      render: (val: string) => new Date(val).toLocaleDateString(),
    },
    {
      title: 'Действия',
      key: 'actions',
      render: (_: unknown, record: any) => (
        <Space>
          <Button
            icon={<EditOutlined />}
            size="small"
            onClick={() => navigate(`/announcements/${record.announcementId}`, { state: { startEditing: true } })}
          />
          <Popconfirm
            title="Удалить?"
            onConfirm={() => deleteAnnouncementMutation.mutate(record.announcementId)}
          >
            <Button icon={<DeleteOutlined />} size="small" danger loading={deleteAnnouncementMutation.isPending} />
          </Popconfirm>
        </Space>
      ),
    },
  ];

  const tabItems = [
    {
      key: 'profile',
      label: 'Профиль',
      children: (
        <Card style={{ maxWidth: 600, margin: '0 auto' }}>
          <Descriptions column={1} bordered size="small">
            <Descriptions.Item label="Имя пользователя">{user.userName}</Descriptions.Item>
            <Descriptions.Item label="Полное имя">{user.fullName}</Descriptions.Item>
            <Descriptions.Item label="Роль">
              <Tag color="blue">{userRoleLabels[user.role as UserRole] ?? user.role}</Tag>
            </Descriptions.Item>
            <Descriptions.Item label="Телефон">
              {editingPhone ? (
                <Space>
                  <Input value={phoneValue} onChange={(e) => setPhoneValue(e.target.value)} size="small" style={{ width: 180 }} />
                  <Button type="primary" size="small" icon={<CheckOutlined />} onClick={handleSavePhone} loading={updatePhoneMutation.isPending} />
                  <Button size="small" icon={<CloseOutlined />} onClick={handleCancelPhone} />
                </Space>
              ) : (
                <Space>
                  <span>{user.phoneNumber}</span>
                  <Button type="text" size="small" icon={<EditOutlined />} onClick={handleEditPhone} />
                </Space>
              )}
            </Descriptions.Item>
            <Descriptions.Item label="Создан">{new Date(user.createdAt).toLocaleDateString()}</Descriptions.Item>
          </Descriptions>
          <Button type="primary" danger onClick={logout} style={{ marginTop: 24 }}>Выйти</Button>
        </Card>
      ),
    },
    {
      key: 'announcements',
      label: `Мои объявления${myAnnouncements ? ` (${myAnnouncements.length})` : ''}`,
      children: (
        <Table
          dataSource={myAnnouncements ?? []}
          columns={announcementColumns}
          rowKey="announcementId"
          loading={announcementsLoading}
          pagination={false}
          size="small"
        />
      ),
    },
    {
      key: 'incoming',
      label: `Входящие запросы${pendingIncoming.length ? ` (${pendingIncoming.length})` : ''}`,
      children: (
        <Table
          dataSource={pendingIncoming}
          columns={incomingColumns}
          rowKey="purchaseRequestId"
          loading={incomingLoading}
          pagination={false}
          size="small"
        />
      ),
    },
    {
      key: 'outgoing',
      label: `Мои запросы${pendingOutgoing.length ? ` (${pendingOutgoing.length})` : ''}`,
      children: (
        <Table
          dataSource={pendingOutgoing}
          columns={outgoingColumns}
          rowKey="purchaseRequestId"
          loading={outgoingLoading}
          pagination={false}
          size="small"
        />
      ),
    },
    {
      key: 'history',
      label: `История${historyRequests.length ? ` (${historyRequests.length})` : ''}`,
      children: (
        <Table
          dataSource={historyRequests}
          columns={historyColumns}
          rowKey="purchaseRequestId"
          loading={incomingLoading || outgoingLoading}
          pagination={false}
          size="small"
        />
      ),
    },
  ];

  return (
    <Card style={{ minHeight: 'calc(100vh - 104px)' }}>
      <Title level={4} style={{ marginBottom: 24 }}>Профиль</Title>
      <Tabs activeKey={activeTab} items={tabItems} onChange={handleTabChange} />
    </Card>
  );
}
