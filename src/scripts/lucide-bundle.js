import {
  createIcons,
  Home,
  FileText,
  Send,
  ClipboardList,
  Users,
  Settings,
  Book,
  LifeBuoy,
  Building2,
  ChevronLeft,
  ChevronRight,
  Menu,
  X,
  Eye,
  EyeOff,
  AlertTriangle,
  CheckCircle,
  Clock,
  Download,
  Edit,
  Trash2,
  Plus,
  Search,
  Filter
} from 'lucide';

// Export createIcons for global use
window.LucideIcons = {
  createIcons,
  icons: {
    Home,
    FileText,
    Send,
    ClipboardList,
    Users,
    Settings,
    Book,
    LifeBuoy,
    Building2,
    ChevronLeft,
    ChevronRight,
    Menu,
    X,
    Eye,
    EyeOff,
    AlertTriangle,
    CheckCircle,
    Clock,
    Download,
    Edit,
    Trash2,
    Plus,
    Search,
    Filter
  }
};

// Re-initialize when DOM changes (for Blazor component updates)
let reinitTimeout;
const observer = new MutationObserver(() => {
  // Debounce icon re-initialization to avoid excessive calls
  clearTimeout(reinitTimeout);
  reinitTimeout = setTimeout(() => {
    console.log('[Lucide] Re-initializing icons due to DOM change');
    window.LucideIcons.createIcons({
      icons: window.LucideIcons.icons
    });
  }, 50);
});

// Start observing immediately after bundle loads
console.log('[Lucide] Starting mutation observer');
observer.observe(document.body, {
  childList: true,
  subtree: true
});

// Initialize on DOMContentLoaded
if (document.readyState === 'loading') {
  document.addEventListener('DOMContentLoaded', () => {
    console.log('[Lucide] Initializing icons on DOMContentLoaded');
    window.LucideIcons.createIcons({
      icons: window.LucideIcons.icons
    });
  });
} else {
  // If DOM is already loaded (bundle loaded after DOM ready)
  console.log('[Lucide] DOM already loaded, initializing icons now');
  window.LucideIcons.createIcons({
    icons: window.LucideIcons.icons
  });
}

console.log('[Lucide] Bundle loaded and ready');
