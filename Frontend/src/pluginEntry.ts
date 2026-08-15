import IpInfoCard from './views/IpInfoCard.vue';
import './style.css';

export const pluginConfig = {
    name: 'IpInfoPlugin',
    version: '1.0.0',

    // 注入路由
    routes: [],
    
    // 注入组件
    extensions: [
        {
            slot: 'dashboard-index-after-system-status', // 仪表盘 - 系统状态监控卡片下方插槽
            component: IpInfoCard,
        }
    ]
};