import { useState, useRef, useEffect } from 'react';
import { Card, Table, Button, Modal, Form, Input, message, Typography, Space, Popconfirm, Pagination } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined, SearchOutlined } from '@ant-design/icons';
import { useAuth } from '../contexts/AuthContext';
import { useOrganizations, useCreateOrganization, useUpdateOrganization, useDeleteOrganization } from '../hooks/useOrganizations';
import { UserRole, type OrganizationDTO } from '../types';

const { Title } = Typography;

export default function OrganizationsPage() {
  const { user: currentUser } = useAuth();
  const isAdmin = (currentUser?.role ?? 0) >= UserRole.Admin;

  const { data: organizations, isLoading } = useOrganizations();
  const createMutation = useCreateOrganization();
  const updateMutation = useUpdateOrganization();
  const deleteMutation = useDeleteOrganization();

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

  const [search, setSearch] = useState('');
  const [current, setCurrent] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [modalOpen, setModalOpen] = useState(false);
  const [editingOrg, setEditingOrg] = useState<OrganizationDTO | null>(null);
  const [form] = Form.useForm();

  const filteredOrgs = (organizations ?? []).filter((o) =>
    o.name.toLowerCase().includes(search.toLowerCase()),
  );

  const paginatedData = filteredOrgs.slice(
    (current - 1) * pageSize,
    current * pageSize,
  );

  useEffect(() => {
    setCurrent(1);
  }, [filteredOrgs.length]);

  const openCreate = () => {
    setEditingOrg(null);
    form.resetFields();
    setModalOpen(true);
  };

  const openEdit = (org: OrganizationDTO) => {
    setEditingOrg(org);
    form.setFieldsValue(org);
    setModalOpen(true);
  };

  const handleSubmit = async () => {
    try {
      const values = await form.validateFields();
      if (editingOrg) {
        await updateMutation.mutateAsync({ ...editingOrg, ...values });
        message.success('Организация обновлена');
      } else {
        await createMutation.mutateAsync(values);
        message.success('Организация создана');
      }
      setModalOpen(false);
      form.resetFields();
    } catch (err: unknown) {
      if (err && typeof err === 'object' && 'errorFields' in err) return;
      const apiError = err as { response?: { data?: { error?: string } } };
      message.error(apiError?.response?.data?.error || 'Ошибка');
    }
  };

  const handleDelete = async (id: string) => {
    try {
      await deleteMutation.mutateAsync(id);
      message.success('Организация удалена');
    } catch {
      message.error('Ошибка при удалении');
    }
  };

  const columns = [
    { title: 'Название', dataIndex: 'name', key: 'name' },
    { title: 'Адрес', dataIndex: 'address', key: 'address' },
    { title: 'Телефон', dataIndex: 'phoneNumber', key: 'phoneNumber' },
    { title: 'Email', dataIndex: 'email', key: 'email' },
    {
      title: 'Создана',
      dataIndex: 'createdAt',
      key: 'createdAt',
      render: (val: string) => new Date(val).toLocaleDateString(),
    },
    ...(isAdmin ? [{
      title: 'Действия',
      key: 'actions',
      render: (_: unknown, record: OrganizationDTO) => (
        <span style={{ whiteSpace: 'nowrap' }}>
          <EditOutlined
            style={{ cursor: 'pointer', marginRight: 12 }}
            onClick={() => openEdit(record)}
          />
          <Popconfirm
            title="Удалить организацию?"
            description="Пользователи организации останутся в системе"
            onConfirm={() => handleDelete(record.organizationId)}
          >
            <DeleteOutlined style={{ cursor: 'pointer', color: '#ff4d4f' }} />
          </Popconfirm>
        </span>
      ),
    }] : []),
  ];

  return (
    <>
      <Card
        style={{ height: 'calc(100vh - 104px)' }}
        styles={{ body: { height: '100%', display: 'flex', flexDirection: 'column', padding: 24 } }}
      >
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 16, flexWrap: 'wrap', gap: 8 }}>
          <Title level={4} style={{ margin: 0 }}>Организации</Title>
          <Space>
            <Input
              placeholder="Поиск по названию..."
              prefix={<SearchOutlined />}
              value={search}
              onChange={(e) => { setSearch(e.target.value); setCurrent(1); }}
              style={{ width: 240 }}
              allowClear
            />
            {isAdmin && (
              <Button type="primary" icon={<PlusOutlined />} onClick={openCreate}>
                Создать организацию
              </Button>
            )}
          </Space>
        </div>

        <div
          ref={tableWrapperRef}
          style={{ flex: 1, minHeight: 0, overflow: 'hidden' }}
        >
          <Table
            dataSource={paginatedData}
            columns={columns}
            rowKey="organizationId"
            loading={isLoading}
            pagination={false}
            scroll={{ y: scrollY }}
          />
        </div>

        <div style={{ flexShrink: 0, display: 'flex', justifyContent: 'flex-end', paddingTop: 16 }}>
          <Pagination
            current={current}
            pageSize={pageSize}
            total={filteredOrgs.length}
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

      <Modal
        title={editingOrg ? 'Редактировать организацию' : 'Создать организацию'}
        open={modalOpen}
        onOk={handleSubmit}
        onCancel={() => { setModalOpen(false); form.resetFields(); }}
        confirmLoading={createMutation.isPending || updateMutation.isPending}
      >
        <Form form={form} layout="vertical" style={{ marginTop: 16 }}>
          <Form.Item name="name" label="Название" rules={[{ required: true, message: 'Введите название' }]}>
            <Input />
          </Form.Item>
          <Form.Item name="address" label="Адрес" rules={[{ required: true, message: 'Введите адрес' }]}>
            <Input />
          </Form.Item>
          <Form.Item name="phoneNumber" label="Телефон" rules={[{ required: true, message: 'Введите телефон' }]}>
            <Input />
          </Form.Item>
          <Form.Item
            name="email"
            label="Email"
            rules={[
              { required: true, message: 'Введите email' },
              { type: 'email', message: 'Некорректный email' },
            ]}
          >
            <Input />
          </Form.Item>
        </Form>
      </Modal>
    </>
  );
}
