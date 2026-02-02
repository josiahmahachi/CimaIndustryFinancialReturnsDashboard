import * as esbuild from 'esbuild';

const isProduction = process.env.NODE_ENV === 'production';

try {
  const result = await esbuild.build({
    entryPoints: ['src/scripts/lucide-bundle.js'],
    bundle: true,
    minify: isProduction,
    sourcemap: !isProduction,
    outfile: 'blazor-wasm-ui/wwwroot/js/lucide.bundle.js',
    format: 'iife',
    target: 'es2020',
    logLevel: 'info'
  });

  console.log('✓ Lucide bundle built successfully');
  console.log(`  Output: blazor-wasm-ui/wwwroot/js/lucide.bundle.js`);
} catch (error) {
  console.error('✗ Bundle build failed:', error);
  process.exit(1);
}
