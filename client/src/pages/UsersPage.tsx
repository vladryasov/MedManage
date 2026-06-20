import { useState, useRef, useEffect } from 'react';
import { Card, Table, Tag, Select, message, Typography, Pagination, Space, Button, Input } from 'antd';
import { EditOutlined, CheckOutlined, CloseOutlined } from '@ant-design/icons';
import { useAuth } from '../contexts/AuthContext';
import { useUsers, useUpdateUserRole, useUpdateUserPhone } from '../hooks/useUsers';
import { UserRole, userRoleLabels, type UserDTO } from '../types';

const { Title } = Typography;

const roleColors: Record<number, string> = {
  [UserRole.All]: 'default',
  [UserRole.CommonUser]: 'blue',
  [UserRole.Admin]: 'red',
  [UserRole.SpecialUser]: 'purple',
};

export default function UsersPage() {
  const [current, setCurrent] = useState(1);
  const [pageSize, setPageSize] = useState(10);

  const tableWrapperRef = useRef<HTMLDivElement>(null);
  const [scrollY, setScrollY] = useState(400);

  useEffect(() => {
    const updateScrollY = () => {
      if (tableWrapperRef.current) {
        setScrollY(Math.max(200, tableWrapperRef.current.clientHeight - 46));
      }
    };
    updateScrollY();
    window.addEventListener('resize', updateScrollY);
    return () => window.removeEventListener('resize', updateScrollY);
  }, []);

  const { data: users, isLoading } = useUsers();
  const updateRoleMutation = useUpdateUserRole();
  const updatePhoneMutation = useUpdateUserPhone();
  const { user: currentUser } = useAuth();
  const isAdmin = (currentUser?.role ?? 0) >= UserRole.Admin;

  const [editingPhoneId, setEditingPhoneId] = useState<string | null>(null);
  const [editingPhoneValue, setEditingPhoneValue] = useState('');

  const handleSavePhone = async (record: UserDTO) => {
    try {
      const updated = { ...record, phoneNumber: editingPhoneValue };
      await updatePhoneMutation.mutateAsync(updated);
      message.success('Номер телефона обновлён');
      setEditingPhoneId(null);
    } catch {
      message.error('Ошибка при обновлении номера');
    }
  };

  const paginatedData = (users ?? []).slice(
    (current - 1) * pageSize,
    current * pageSize,
  );

  useEffect(() => {
    setCurrent(1);
  }, [users?.length]);

  const handleRoleChange = (userId: string, newRole: UserRole) => {
    const user = users?.find((u) => u.userId === userId);
    if (!user) return;
    updateRoleMutation.mutate(
      { user, newRole },
      { onSuccess: () => message.success('Роль обновлена') },
    );
  };

  const columns = [
    { title: 'Имя пользователя', dataIndex: 'userName', key: 'userName' },
    { title: 'Полное имя', dataIndex: 'fullName', key: 'fullName' },
    {
      title: 'Роль',
      dataIndex: 'role',
      key: 'role',
      render: (role: UserRole, record: UserDTO) =>
        isAdmin ? (
          <Select
            value={role}
            onChange={(val: UserRole) => handleRoleChange(record.userId, val)}
            style={{ width: 160 }}
          >
            {(
              Object.entries(userRoleLabels) as [string, string][]
            ).filter(([value]) => {
              const numVal = Number(value);
              return numVal !== UserRole.All && numVal <= (currentUser?.role ?? 0);
            }).map(([value, label]) => (
              <Select.Option key={value} value={Number(value)}>
                {label}
              </Select.Option>
            ))}
          </Select>
        ) : (
          <span>{userRoleLabels[role] ?? role}</span>
        ),
    },
    {
      title: 'Роль (тег)',
      dataIndex: 'role',
      key: 'roleTag',
      render: (role: UserRole) => (
        <Tag color={roleColors[role] ?? 'default'}>
          {userRoleLabels[role] ?? role}
        </Tag>
      ),
    },
    {
      title: 'Телефон',
      dataIndex: 'phoneNumber',
      key: 'phoneNumber',
      render: (phone: string, record: UserDTO) =>
        isAdmin && editingPhoneId === record.userId ? (
          <Space>
            <Input
              size="small"
              value={editingPhoneValue}
              onChange={(e) => setEditingPhoneValue(e.target.value)}
              style={{ width: 140 }}
            />
            <Button
              size="small"
              type="primary"
              icon={<CheckOutlined />}
              onClick={() => handleSavePhone(record)}
              loading={updatePhoneMutation.isPending}
            />
            <Button
              size="small"
              icon={<CloseOutlined />}
              onClick={() => setEditingPhoneId(null)}
            />
          </Space>
        ) : (
          <Space>
            <span>{phone}</span>
            {isAdmin && (
              <Button
                type="link"
                size="small"
                icon={<EditOutlined />}
                onClick={() => {
                  setEditingPhoneId(record.userId);
                  setEditingPhoneValue(record.phoneNumber);
                }}
              />
            )}
          </Space>
        ),
    },
    {
      title: 'Создан',
      dataIndex: 'createdAt',
      key: 'createdAt',
      render: (val: string) => new Date(val).toLocaleDateString(),
    },
  ];

  return (
    <Card
      style={{ height: 'calc(100vh - 104px)' }}
      styles={{ body: { height: '100%', display: 'flex', flexDirection: 'column', padding: 24 } }}
    >
      <Title level={4}>Пользователи</Title>

      <div
        ref={tableWrapperRef}
        style={{ flex: 1, minHeight: 0, overflow: 'hidden' }}
      >
        <Table
          dataSource={paginatedData}
          columns={columns}
          rowKey="userId"
          loading={isLoading}
          pagination={false}
          scroll={{ y: scrollY }}
        />
      </div>

      <div style={{ flexShrink: 0, display: 'flex', justifyContent: 'flex-end', paddingTop: 16 }}>
        <Pagination
          current={current}
          pageSize={pageSize}
          total={users?.length ?? 0}
          showSizeChanger
          pageSizeOptions={['10', '20', '50', '100']}
          showTotal={(total, range) => `${range[0]}-${range[1]} из ${total}`}
          onChange={(page: number) => setCurrent(page)}
          onShowSizeChange={(_: number, size: number) => {
            setPageSize(size);
            setCurrent(1);
          }}
        />
      </div>
    </Card>
  );
}
