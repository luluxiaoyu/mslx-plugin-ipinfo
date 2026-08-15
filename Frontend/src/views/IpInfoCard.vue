<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { Tag as TTag, Button as TButton, MessagePlugin } from 'tdesign-vue-next';
import { fetchIpSummary } from '../api/ip';
import type { IpSummaryResponse } from '../api/ip';
import IpDetailDialog from './IpDetailDialog.vue';

const summary = ref<IpSummaryResponse | null>(null);
const loading = ref(false);
const detailDialogRef = ref<InstanceType<typeof IpDetailDialog> | null>(null);

const loadSummary = async (forceRefresh = false) => {
  loading.value = true;
  try {
    summary.value = await fetchIpSummary(forceRefresh);
    if (forceRefresh) {
      MessagePlugin.success('刷新成功');
    }
  } catch (error) {
    MessagePlugin.error('获取 IP 概要信息失败');
  } finally {
    loading.value = false;
  }
};

const copyText = (text: string | undefined) => {
  if (!text) return;
  navigator.clipboard.writeText(text).then(() => {
    MessagePlugin.success('已复制到剪贴板');
  }).catch(() => {
    MessagePlugin.error('复制失败');
  });
};

const openDetails = () => {
  if (detailDialogRef.value) {
    detailDialogRef.value.open();
  }
};

onMounted(() => {
  loadSummary();
});
</script>

<template>
  <div class="design-card w-full bg-white dark:bg-zinc-800 p-4 sm:p-5 rounded-2xl border border-[var(--td-component-border)] shadow-sm transition-all duration-300">
    <!-- Header -->
    <div class="flex items-center justify-between mb-4">
      <div class="flex items-center gap-2">
        <i class="fa-solid fa-network-wired text-[var(--color-primary)]"></i>
        <span class="font-bold text-[var(--td-text-color-primary)]">网络 IP 信息</span>
      </div>
      <div class="flex items-center gap-2">
        <t-button variant="text" shape="square" :loading="loading" @click="loadSummary(true)">
          <template #icon><i class="fa-solid fa-rotate-right"></i></template>
        </t-button>
        <t-button variant="outline" size="small" @click="openDetails">
          网卡详情
        </t-button>
      </div>
    </div>

    <!-- Content -->
    <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
      
      <!-- IPv4 Card -->
      <div class="flex flex-col justify-center p-3 rounded-xl bg-zinc-50/50 dark:bg-zinc-900/30 border border-zinc-200/80 dark:border-zinc-700/60 hover:border-zinc-300 dark:hover:border-zinc-500 hover:bg-zinc-100/50 dark:hover:bg-zinc-800/40 transition-all group relative overflow-hidden">
        <div class="text-[13px] text-[var(--td-text-color-secondary)] font-medium mb-1">公网 IPv4</div>
        <div class="flex items-center gap-2">
          <span v-if="loading" class="text-xl font-bold font-mono text-[var(--td-text-color-placeholder)]">获取中...</span>
          <span v-else-if="summary?.externalIpv4" class="text-xl font-bold font-mono text-[var(--td-text-color-primary)]">{{ summary.externalIpv4 }}</span>
          <span v-else class="text-xl font-bold font-mono text-[var(--td-text-color-placeholder)]">无 / 未获取</span>
          
          <i v-if="summary?.externalIpv4 && !loading" class="fa-solid fa-copy cursor-pointer text-[var(--td-text-color-secondary)] hover:text-[var(--color-primary)] opacity-0 group-hover:opacity-100 transition-opacity" @click="copyText(summary.externalIpv4)"></i>
        </div>
        <t-tag v-if="summary?.externalIpv4 && !loading" theme="success" shape="round" size="small" variant="light" class="w-fit mt-2 absolute right-3 top-3">在线</t-tag>
      </div>

      <!-- IPv6 Card -->
      <div class="flex flex-col justify-center p-3 rounded-xl bg-zinc-50/50 dark:bg-zinc-900/30 border border-zinc-200/80 dark:border-zinc-700/60 hover:border-zinc-300 dark:hover:border-zinc-500 hover:bg-zinc-100/50 dark:hover:bg-zinc-800/40 transition-all group relative overflow-hidden">
        <div class="text-[13px] text-[var(--td-text-color-secondary)] font-medium mb-1">公网 IPv6</div>
        <div class="flex items-center gap-2">
          <span v-if="loading" class="text-xl font-bold font-mono text-[var(--td-text-color-placeholder)]">获取中...</span>
          <span v-else-if="summary?.externalIpv6" class="text-xl font-bold font-mono text-[var(--td-text-color-primary)]">{{ summary.externalIpv6 }}</span>
          <span v-else class="text-xl font-bold font-mono text-[var(--td-text-color-placeholder)]">无 / 未分配</span>
          
          <i v-if="summary?.externalIpv6 && !loading" class="fa-solid fa-copy cursor-pointer text-[var(--td-text-color-secondary)] hover:text-[var(--color-primary)] opacity-0 group-hover:opacity-100 transition-opacity" @click="copyText(summary.externalIpv6)"></i>
        </div>
        <t-tag v-if="summary?.externalIpv6 && !loading" theme="primary" shape="round" size="small" variant="light" class="w-fit mt-2 absolute right-3 top-3">支持</t-tag>
      </div>

    </div>
  </div>
  
  <IpDetailDialog ref="detailDialogRef" />
</template>

<style scoped>
@reference "@/style/tailwind/index.css";
</style>
