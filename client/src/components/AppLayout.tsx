import { Layout, Menu, Button, Dropdown, theme, Badge } from 'antd';
import { Outlet, useNavigate, useLocation } from 'react-router-dom';
import {
  FileTextOutlined,
  UserOutlined,
  LogoutOutlined,
  TeamOutlined,
  BankOutlined,
  BellOutlined,
  MenuFoldOutlined,
  MenuUnfoldOutlined,
  SunOutlined,
  MoonOutlined,
} from '@ant-design/icons';
import { useState } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { useTheme } from '../contexts/ThemeContext';
import { useUnreadCount, useNotifications } from '../hooks/useNotifications';
import { useMarkAsRead } from '../hooks/useNotifications';
import { InAppNotificationType, type InAppNotificationDTO } from '../types';
import styles from './AppLayout.module.css';

const { Header, Sider, Content } = Layout;

export default function AppLayout() {
  const navigate = useNavigate();
  const location = useLocation();
  const { user, logout } = useAuth();
  const { isDarkMode, toggleTheme } = useTheme();
  const { token } = theme.useToken();
  const [collapsed, setCollapsed] = useState(false);
  const { data: unreadCount } = useUnreadCount();
  const { data: notifications } = useNotifications();
  const markAsReadMutation = useMarkAsRead();

  const menuItems = [
    {
      key: '/',
      icon: <FileTextOutlined />,
      label: 'Объявления',
    },
    {
      key: '/users',
      icon: <TeamOutlined />,
      label: 'Пользователи',
    },
    {
      key: '/organizations',
      icon: <BankOutlined />,
      label: 'Организации',
    },
  ];

  const selectedKey = menuItems
    .map((item) => item.key)
    .includes(location.pathname)
    ? location.pathname
    : '/';

  const userMenu = {
    items: [
      {
        key: 'profile',
        icon: <UserOutlined />,
        label: 'Профиль',
        onClick: () => navigate('/profile'),
      },
      {
        key: 'notifications',
        icon: <BellOutlined />,
        label: 'Уведомления',
        onClick: () => navigate('/notifications'),
      },
      {
        key: 'logout',
        icon: <LogoutOutlined />,
        label: 'Выйти',
        onClick: logout,
      },
    ],
  };

  const getNotificationTab = (n: InAppNotificationDTO) => {
    if (n.type === InAppNotificationType.PurchaseRequest) return 'incoming';
    return 'history';
  };

  const notificationDropdownItems = (notifications ?? []).slice(0, 5).map((n) => ({
    key: n.id,
    label: (
      <div
        style={{
          maxWidth: 300,
          cursor: 'pointer',
          color: n.isRead ? token.colorTextTertiary : token.colorText,
        }}
        onClick={() => {
          if (!n.isRead) {
            markAsReadMutation.mutate(n.id);
          }
          navigate(`/profile?tab=${getNotificationTab(n)}`);
        }}
      >
        <div style={{ fontWeight: n.isRead ? 'normal' : 'bold', fontSize: 13 }}>{n.title}</div>
        <div style={{ fontSize: 12, color: token.colorTextSecondary }}>{n.message}</div>
      </div>
    ),
  }));

  if (notifications && notifications.length > 5) {
    notificationDropdownItems.push({
      key: 'all',
      label: (
        <div style={{ textAlign: 'center' }} onClick={() => navigate('/notifications')}>
          Все уведомления
        </div>
      ),
    });
  }

  return (
    <Layout className={styles.layout}>
      <Sider
        collapsible 
        collapsed={collapsed}
        onCollapse={setCollapsed}
        theme={isDarkMode ? 'dark' : 'light'}
      >
        <div
          style={{
            height: 64,
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            fontWeight: 'bold',
            fontSize: collapsed ? 14 : 18,
            borderBottom: `1px solid ${token.colorBorderSecondary}`,
          }}
        >
          {collapsed ? 'MM' : 'MedManage'}
        </div>
        <Menu
          mode="inline"
          selectedKeys={[selectedKey]}
          items={menuItems}
          onClick={({ key }) => navigate(key)}
        />
      </Sider>
      <Layout>
        <Header
          style={{
            background: token.colorBgContainer,
            borderBottom: `1px solid ${token.colorBorderSecondary}`,
          }}
          className={styles.header}
        >
          <div className={styles.headerLeft}>
            <Button
              type="text"
              icon={collapsed ? <MenuUnfoldOutlined /> : <MenuFoldOutlined />}
              onClick={() => setCollapsed(!collapsed)}
            />
          </div>
          <div className={styles.headerRight}>
            <Button
              type="text"
              icon={isDarkMode ? <SunOutlined /> : <MoonOutlined />}
              onClick={toggleTheme}
            />
            <Dropdown menu={{ items: notificationDropdownItems }} trigger={['click']}>
              <Badge count={unreadCount ?? 0} size="small">
                <Button type="text" icon={<BellOutlined />} />
              </Badge>
            </Dropdown>
            <Dropdown menu={userMenu}>
              <Button type="text" icon={<UserOutlined />}>
                {user?.fullName ?? user?.userName ?? 'Пользователь'}
              </Button>
            </Dropdown>
          </div>
        </Header>
        <Content className={styles.content}>
          <Outlet />
        </Content>
      </Layout>
    </Layout>
  );
}
