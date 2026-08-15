<script setup lang="ts">
import { ref } from 'vue';
import { Dialog as TDialog, Tag as TTag, MessagePlugin } from 'tdesign-vue-next';
import { fetchIpDetails } from '../api/ip';
import type { NetworkAdapterDetail } from '../api/ip';

const visible = ref(false);
const adapters = ref<NetworkAdapterDetail[]>([]);
const loading = ref(false);

const open = async () => {
  visible.value = true;
  await loadData();
};

const loadData = async () => {
  loading.value = true;
  try {
    adapters.value = await fetchIpDetails();
  } catch (error) {
    MessagePlugin.error('获取网卡详情失败');
  } finally {
    loading.value = false;
  }
};

const handleClose = () => {
  visible.value = false;
};

const copyText = (text: string) => {
  if (!text) return;
  navigator.clipboard.writeText(text).then(() => {
    MessagePlugin.success('已复制到剪贴板');
  }).catch(() => {
    MessagePlugin.error('复制失败');
  });
};

defineExpose({
  open
});
</script>

<template>
  <t-dialog v-model:visible="visible" header="宿主机网卡与 IP 详情" width="800px" :footer="false" @close="handleClose">
    <div v-if="loading" class="flex justify-center items-center h-40">
      <div class="text-[var(--td-text-color-secondary)]">正在加载数据...</div>
    </div>
    
    <div v-else class="flex flex-col gap-4 max-h-[60vh] overflow-y-auto pr-2 custom-scrollbar">
      <div v-for="(adapter, index) in adapters" :key="index" class="p-4 bg-zinc-50 dark:bg-zinc-800/50 rounded-xl border border-zinc-200 dark:border-zinc-700">
        <div class="flex items-center justify-between mb-3">
          <div class="flex items-center gap-2">
            <span class="font-bold text-[var(--td-text-color-primary)]">{{ adapter.name }}</span>
            <t-tag :theme="adapter.status === 'Up' ? 'success' : 'default'" size="small" variant="light-outline">
              {{ adapter.status }}
            </t-tag>
          </div>
          <div class="text-xs text-[var(--td-text-color-secondary)]">{{ adapter.type }}</div>
        </div>
        <div class="text-xs text-[var(--td-text-color-secondary)] mb-3">{{ adapter.description }}</div>
        
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <!-- MAC & Misc -->
          <div class="flex flex-col gap-2">
            <div class="text-sm font-semibold text-[var(--td-text-color-primary)]">基础信息</div>
            <div class="flex items-center justify-between p-2 rounded-lg bg-white dark:bg-zinc-800 border border-[var(--td-component-border)]">
              <span class="text-xs text-[var(--td-text-color-secondary)]">MAC 地址</span>
              <div class="flex items-center gap-2">
                <span class="font-mono text-sm text-[var(--td-text-color-primary)]">{{ adapter.macAddress || '无' }}</span>
                <i v-if="adapter.macAddress" class="fa-solid fa-copy cursor-pointer text-[var(--td-text-color-secondary)] hover:text-[var(--color-primary)]" @click="copyText(adapter.macAddress)"></i>
              </div>
            </div>
          </div>

          <!-- IPv4 -->
          <div class="flex flex-col gap-2">
            <div class="text-sm font-semibold text-[var(--td-text-color-primary)]">IPv4 地址</div>
            <template v-if="adapter.ipv4Addresses.length > 0">
              <div v-for="(ip, i) in adapter.ipv4Addresses" :key="'v4-'+i" class="flex items-center justify-between p-2 rounded-lg bg-white dark:bg-zinc-800 border border-[var(--td-component-border)]">
                <div class="flex flex-col">
                  <span class="font-mono text-sm text-[var(--td-text-color-primary)]">{{ ip.address }}</span>
                  <span class="text-[10px] text-[var(--td-text-color-secondary)]">{{ ip.prefixOrMask }}</span>
                </div>
                <i class="fa-solid fa-copy cursor-pointer text-[var(--td-text-color-secondary)] hover:text-[var(--color-primary)]" @click="copyText(ip.address)"></i>
              </div>
            </template>
            <div v-else class="text-xs text-[var(--td-text-color-secondary)] p-2">无 IPv4 地址</div>
          </div>

          <!-- IPv6 -->
          <div class="flex flex-col gap-2 md:col-span-2">
            <div class="text-sm font-semibold text-[var(--td-text-color-primary)]">IPv6 地址</div>
            <template v-if="adapter.ipv6Addresses.length > 0">
              <div class="grid grid-cols-1 md:grid-cols-2 gap-2">
                <div v-for="(ip, i) in adapter.ipv6Addresses" :key="'v6-'+i" class="flex items-center justify-between p-2 rounded-lg bg-white dark:bg-zinc-800 border border-[var(--td-component-border)]">
                  <div class="flex flex-col overflow-hidden mr-2">
                    <span class="font-mono text-sm text-[var(--td-text-color-primary)] truncate" :title="ip.address">{{ ip.address }}</span>
                    <div class="flex items-center gap-1">
                      <span class="text-[10px] text-[var(--td-text-color-secondary)]">/{{ ip.prefixOrMask }}</span>
                      <t-tag size="small" :theme="ip.type === 'Global Unicast' ? 'primary' : 'default'" variant="light">{{ ip.type }}</t-tag>
                    </div>
                  </div>
                  <i class="fa-solid fa-copy cursor-pointer text-[var(--td-text-color-secondary)] hover:text-[var(--color-primary)] shrink-0" @click="copyText(ip.address)"></i>
                </div>
              </div>
            </template>
            <div v-else class="text-xs text-[var(--td-text-color-secondary)] p-2">无 IPv6 地址</div>
          </div>
        </div>
      </div>
    </div>
  </t-dialog>
</template>

<style scoped>
.custom-scrollbar::-webkit-scrollbar {
  width: 6px;
}
.custom-scrollbar::-webkit-scrollbar-thumb {
  background: var(--td-scrollbar-color);
  border-radius: 4px;
}
</style>
