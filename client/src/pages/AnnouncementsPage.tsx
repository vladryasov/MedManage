import { useState } from 'react';
import { usePaginatedAnnouncements } from '../hooks/useAnnouncements';
import {
  Card,
  Table,
  Tag,
  Button,
  Input,
  Select,
  Space,
  Typography,
  Popconfirm,
  message,
  Pagination,
} from 'antd';
import { PlusOutlined, SearchOutlined, DeleteOutlined, EditOutlined } from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { deleteAnnouncement } from '../api/announcements';
import { useAuth } from '../contexts/AuthContext';
import {
  InventoryStatus,
  ProductType,
  inventoryStatusLabels,
  productTypeLabels,
  UserRole,
  type AnnouncementDTO,
} from '../types';

const { Title } = Typography;

const statusColors: Record<number, string> = {
  [InventoryStatus.All]: 'default',
  [InventoryStatus.InStock]: 'green',
  [InventoryStatus.OutOfStock]: 'red',
  [InventoryStatus.Replenishing]: 'blue',
  [InventoryStatus.Expired]: 'orange',
};

export default function AnnouncementsPage() {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const { user } = useAuth();
  const isAdmin = (user?.role ?? 0) >= UserRole.Admin;

  const [searchFilter, setSearchFilter] = useState('');
  const [productTypeFilter, setProductTypeFilter] = useState<ProductType>(ProductType.All);
  const [statusFilter, setStatusFilter] = useState<InventoryStatus>(InventoryStatus.All);
  const [current, setCurrent] = useState(1);
  const [pageSize, setPageSize] = useState(10);

  const { data, isLoading } = usePaginatedAnnouncements({
    pageNumber: current,
    pageSize,
    searchFilter: searchFilter || undefined,
    productType: productTypeFilter !== ProductType.All ? productTypeFilter : undefined,
    statusInventory: statusFilter !== InventoryStatus.All ? statusFilter : undefined,
  });

  const deleteMutation = useMutation({
    mutationFn: deleteAnnouncement,
    onSuccess: () => {
      message.success('Объявление удалено');
      queryClient.invalidateQueries({ queryKey: ['announcements', 'paginated'] });
    },
  });

  const columns = [
    {
      title: 'Заголовок',
      dataIndex: 'title',
      key: 'title',
    },
    {
      title: 'Автор',
      dataIndex: 'userName',
      key: 'userName',
    },
    {
      title: 'Тип',
      dataIndex: 'typeProduct',
      key: 'typeProduct',
      render: (val: ProductType) => productTypeLabels[val] ?? val,
    },
    {
      title: 'Статус',
      dataIndex: 'statusInventory',
      key: 'statusInventory',
      render: (val: InventoryStatus) => (
        <Tag color={statusColors[val]}>{inventoryStatusLabels[val] ?? val}</Tag>
      ),
    },
    {
      title: 'Просмотры',
      dataIndex: 'views',
      key: 'views',
    },
    ...(isAdmin
      ? [
          {
            title: 'Действия',
            key: 'actions',
            render: (_: unknown, record: AnnouncementDTO) => (
              <span onClick={(e) => e.stopPropagation()} style={{ whiteSpace: 'nowrap' }}>
                <EditOutlined
                  style={{ cursor: 'pointer', marginRight: 12 }}
                  onClick={() => navigate(`/announcements/${record.announcementId}`, { state: { startEditing: true } })}
                />
                <Popconfirm
                  title="Удалить?"
                  onConfirm={() => deleteMutation.mutate(record.announcementId)}
                >
                  <DeleteOutlined style={{ cursor: 'pointer', color: '#ff4d4f' }} />
                </Popconfirm>
              </span>
            ),
          },
        ]
      : []),
  ];

  return (
    <Card
      style={{ height: 'calc(100vh - 104px)' }}
      styles={{ body: { height: '100%', display: 'flex', flexDirection: 'column', padding: 24 } }}
    >
      <div
        style={{
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
          marginBottom: 4,
          flexShrink: 0,
        }}
      >
        <Title level={4} style={{ margin: 0 }}>
          Объявления
        </Title>
        <Button
          type="primary"
          icon={<PlusOutlined />}
          onClick={() => navigate('/announcements/create')}
        >
          Создать
        </Button>
      </div>

      <div style={{ flexShrink: 0, marginBottom: 16 }}>
        <Space wrap>
          <Input
            placeholder="Поиск..."
            prefix={<SearchOutlined />}
            value={searchFilter}
            onChange={(e) => {
              setSearchFilter(e.target.value);
              setCurrent(1);
            }}
            style={{ width: 240 }}
            allowClear
          />
          <Select
            value={productTypeFilter}
            onChange={(v) => { setProductTypeFilter(v); setCurrent(1); }}
            style={{ width: 180 }}
          >
            {Object.entries(productTypeLabels).map(([value, label]) => (
              <Select.Option key={value} value={Number(value)}>
                {label}
              </Select.Option>
            ))}
          </Select>
          <Select
            value={statusFilter}
            onChange={(v) => { setStatusFilter(v); setCurrent(1); }}
            style={{ width: 160 }}
          >
            {Object.entries(inventoryStatusLabels).map(([value, label]) => (
              <Select.Option key={value} value={Number(value)}>
                {label}
              </Select.Option>
            ))}
          </Select>
        </Space>
      </div>

      <div style={{ flex: 1, minHeight: 0, overflow: 'hidden' }}>
        <Table
          dataSource={data?.items ?? []}
          columns={columns}
          rowKey="announcementId"
          loading={isLoading}
          pagination={false}
          scroll={{ y: 550 }}
          onRow={(record: AnnouncementDTO) => ({
            onClick: () => navigate(`/announcements/${record.announcementId}`),
            style: { cursor: 'pointer' },
          })}
        />
      </div>

      <div style={{ flexShrink: 0, display: 'flex', justifyContent: 'flex-end', paddingTop: 16}}>
        <Pagination
          current={current}
          pageSize={pageSize}
          total={data?.totalCount ?? 0}
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
