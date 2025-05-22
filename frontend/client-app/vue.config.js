const { defineConfig } = require('@vue/cli-service')
module.exports = {
  outputDir: 'dist',
  publicPath: '/', // Убедитесь, что Vue работает с корневым путем
  devServer: {
    port: 8050, // Порт для разработки, если нужен
    proxy: {
      '/api': {
        target: 'https://localhost:7095', // Порт вашего API
        changeOrigin: true,
        secure: false
      }
    }
  }
}
