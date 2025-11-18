<template>
  <div style="padding: 20px;">
    <h2>Weather Forecast</h2>
    <button @click="loadWeather">載入天氣資料</button>
    <table v-if="weather.length" border="1" cellspacing="0" cellpadding="6" style="margin-top: 20px;">
      <thead>
        <tr>
          <th>日期</th>
          <th>氣溫 (°C)</th>
          <th>描述</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(item, i) in weather" :key="i">
          <td>{{ item.date }}</td>
          <td>{{ item.temperatureC }}</td>
          <td>{{ item.summary }}</td>
        </tr>
      </tbody>
    </table>
    <p v-else style="margin-top: 20px;">尚未載入資料</p>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import axios from 'axios'

const weather = ref([])
const loadWeather = async () => {
  try {
    const res = await axios.get('https://localhost:44319/WeatherForecast')
    console.log(res.data)  // ✅ 先在 console 印出看看
    weather.value = res.data
  } catch (err) {
    console.error('取得天氣資料失敗：', err)
    alert('無法取得天氣資料')
  }
}
</script>
