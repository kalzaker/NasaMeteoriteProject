<template>
  <div class="container mt-4">
    <h2>Метеориты (по годам)</h2>

    <div class="row mb-3">
      <div class="col">
        <label>Год с:</label>
        <select v-model="filters.yearFrom" class="form-control">
          <option v-for="year in years" :key="year" :value="year">{{ year }}</option>
        </select>
      </div>
      <div class="col">
        <label>Год по:</label>
        <select v-model="filters.yearTo" class="form-control">
          <option v-for="year in years" :key="year" :value="year">{{ year }}</option>
        </select>
      </div>
      <div class="col">
        <label>Класс:</label>
        <select v-model="filters.recclass" class="form-control">
          <option value="">Все</option>
          <option v-for="cls in recclasses" :key="cls" :value="cls">{{ cls }}</option>
        </select>
      </div>
      <div class="col">
        <label>Название содержит:</label>
        <input type="text" class="form-control" v-model="filters.nameContains" />
      </div>
    </div>

    <button class="btn btn-primary mb-3" @click="fetchData">Обновить</button>

    <table class="table table-bordered">
      <thead>
        <tr>
          <th @click="sortBy('year')" style="cursor: pointer">
            Год
            <span v-if="sortField === 'year'">
              {{ sortDirection === 'asc' ? '↑' : '↓' }}
            </span>
          </th>
          <th @click="sortBy('count')" style="cursor: pointer">
            Количество метеоритов
            <span v-if="sortField === 'count'">
              {{ sortDirection === 'asc' ? '↑' : '↓' }}
            </span>
          </th>
          <th @click="sortBy('totalMass')" style="cursor: pointer">
            Суммарная масса
            <span v-if="sortField === 'totalMass'">
              {{ sortDirection === 'asc' ? '↑' : '↓' }}
            </span>
          </th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="row in sortedData" :key="row.year">
          <td>{{ row.year }}</td>
          <td>{{ row.count }}</td>
          <td>{{ row.totalMass }}</td>
        </tr>
        <tr>
          <td><strong>Total</strong></td>
          <td><strong>{{ total.count }}</strong></td>
          <td><strong>{{ total.mass }}</strong></td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script>
import axios from 'axios'

export default {
  name: 'MeteoriteReport',
  data() {
    return {
      data: [],
      recclasses: [],
      years: [],
      allYears: [],
      filters: {
        yearFrom: null,
        yearTo: null,
        recclass: '',
        nameContains: ''
      },
      sortField: 'year',
      sortDirection: 'asc'
    }
  },
  computed: {
    total() {
      return {
        count: this.data.reduce((sum, r) => sum + r.count, 0),
        mass: this.data.reduce((sum, r) => sum + r.totalMass, 0)
      }
    },
    sortedData() {
      return [...this.data].sort((a, b) => {
        const field = this.sortField
        const dir = this.sortDirection === 'asc' ? 1 : -1
        return a[field] > b[field] ? dir : a[field] < b[field] ? -dir : 0
      })
    }
  },
  mounted() {
    this.fetchAllYears()
    this.fetchRecclasses()
    this.fetchData()
  },
  methods: {
    async fetchAllYears() {
      const res = await axios.get('https://localhost:7095/api/meteorites')
      const years = res.data
        .filter(m => m.year)
        .map(m => new Date(m.year).getFullYear())

      this.allYears = [...new Set(years)].sort((a, b) => a - b)
      this.years = this.allYears

      if (!this.filters.yearFrom) this.filters.yearFrom = this.years[0]
      if (!this.filters.yearTo) this.filters.yearTo = this.years[this.years.length - 1]
    },
    async fetchRecclasses() {
      const res = await axios.get('https://localhost:7095/api/meteorites')
      const classes = res.data.map(m => m.recclass)
      this.recclasses = [...new Set(classes)].sort()
    },
    async fetchData() {
      if (this.filters.yearFrom > this.filters.yearTo) {
        alert('Год "с" не может быть больше года "по"')
        return
      }

      const params = { ...this.filters }

      const res = await axios.get('https://localhost:7095/api/meteorites/filtered', { params })
      this.data = res.data
    },
    sortBy(field) {
      if (this.sortField === field) {
        this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc'
      } else {
        this.sortField = field
        this.sortDirection = 'asc'
      }
    }
  }
}
</script>

<style scoped>
th {
  user-select: none;
}
</style>
