import { useMutation, useQueryClient } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import {
  Card,
  Form,
  Input,
  Select,
  DatePicker,
  Button,
  Typography,
  message,
} from 'antd';
import { createAnnouncement } from '../api/announcements';
import {
  InventoryStatus,
  ProductType,
  productTypeLabels,
  inventoryStatusLabels,
} from '../types';

const { Title } = Typography;

export default function CreateAnnouncementPage() {
  const [form] = Form.useForm();
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: createAnnouncement,
    onSuccess: () => {
      message.success('Объявление создано');
      queryClient.invalidateQueries({ queryKey: ['announcements'] });
      navigate('/');
    },
    onError: () => message.error('Ошибка при создании объявления'),
  });

  const onFinish = (values: any) => {
    const payload = {
      title: values.title,
      content: values.content,
      statusInventory: values.statusInventory,
      typeProduct: values.typeProduct,
      expirationDate: values.expirationDate?.toISOString() ?? null,
      userName: '',
      views: 0,
    };
    mutation.mutate(payload);
  };

  return (
    <Card style={{ maxWidth: 640, margin: '24px auto' }}>
      <Title level={4}>Создать объявление</Title>
      <Form form={form} layout="vertical" onFinish={onFinish}>
        <Form.Item name="title" label="Заголовок" rules={[{ required: true }]}>
          <Input maxLength={255} />
        </Form.Item>
        <Form.Item name="content" label="Содержание" rules={[{ required: true }]}>
          <Input.TextArea rows={4} maxLength={2000} />
        </Form.Item>
        <Form.Item
          name="typeProduct"
          label="Тип продукта"
          rules={[{ required: true }]}
          initialValue={ProductType.Tablet}
        >
          <Select>
            {Object.entries(productTypeLabels)
              .filter(([key]) => Number(key) !== ProductType.All)
              .map(([value, label]) => (
                <Select.Option key={value} value={Number(value)}>
                  {label}
                </Select.Option>
              ))}
          </Select>
        </Form.Item>
        <Form.Item
          name="statusInventory"
          label="Статус"
          rules={[{ required: true }]}
          initialValue={InventoryStatus.InStock}
        >
          <Select>
            {Object.entries(inventoryStatusLabels)
              .filter(([key]) => Number(key) !== InventoryStatus.All)
              .map(([value, label]) => (
                <Select.Option key={value} value={Number(value)}>
                  {label}
                </Select.Option>
              ))}
          </Select>
        </Form.Item>
        <Form.Item name="expirationDate" label="Дата истечения">
          <DatePicker style={{ width: '100%' }} />
        </Form.Item>
        <Form.Item>
          <Button type="primary" htmlType="submit" loading={mutation.isPending}>
            Создать
          </Button>
          <Button style={{ marginLeft: 8 }} onClick={() => navigate('/')}>
            Отмена
          </Button>
        </Form.Item>
      </Form>
    </Card>
  );
}
