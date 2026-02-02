const StyleDictionary = require('style-dictionary');

// Custom format for CSS variables
StyleDictionary.registerFormat({
  name: 'css/variables',
  formatter: function({ dictionary, options }) {
    const selector = options.selector || ':root';
    return `${selector} {\n${dictionary.allTokens.map(token => {
      let value = token.value;
      // Handle references
      if (token.original && token.original.value && token.original.value.startsWith('{')) {
        value = token.value;
      }
      return `  --${token.name}: ${value};`;
    }).join('\n')}\n}`;
  }
});

// Custom transform to convert token names to kebab-case
StyleDictionary.registerTransform({
  name: 'name/kebab',
  type: 'name',
  transformer: (token) => {
    return token.path.join('-').toLowerCase();
  }
});

module.exports = {
  source: ['tokens/core/**/*.json'],
  platforms: {
    css: {
      transformGroup: 'css',
      transforms: ['name/kebab', 'color/css'],
      buildPath: '../blazor-wasm-ui/wwwroot/css/tokens/',
      files: [
        {
          destination: 'core.css',
          format: 'css/variables',
          options: {
            selector: ':root'
          }
        }
      ]
    },
    light: {
      source: ['tokens/core/**/*.json', 'tokens/themes/light.json'],
      transformGroup: 'css',
      transforms: ['name/kebab', 'color/css'],
      buildPath: '../blazor-wasm-ui/wwwroot/css/tokens/',
      files: [
        {
          destination: 'light.css',
          format: 'css/variables',
          filter: (token) => token.path[0] === 'semantic',
          options: {
            selector: ':root'
          }
        }
      ]
    },
    dark: {
      source: ['tokens/core/**/*.json', 'tokens/themes/dark.json'],
      transformGroup: 'css',
      transforms: ['name/kebab', 'color/css'],
      buildPath: '../blazor-wasm-ui/wwwroot/css/tokens/',
      files: [
        {
          destination: 'dark.css',
          format: 'css/variables',
          filter: (token) => token.path[0] === 'semantic',
          options: {
            selector: '.dark'
          }
        }
      ]
    }
  }
};
