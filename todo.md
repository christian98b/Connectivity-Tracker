# Connectivity Tracker - TODO List

## Epic 0: UI-Grundgerüst & App-Skelett

### Story 0.1: Hauptfenster (Main Shell)
- [ ] Create main application window with WPF
- [ ] Implement standard Windows controls (Minimize, Maximize, Close)
- [ ] Add application title to title bar
- [ ] Set minimum window size
- [ ] Apply modern design framework styling

### Story 0.2: Navigationsstruktur
- [ ] Implement navigation sidebar/tab structure
- [ ] Create placeholder buttons for Dashboard
- [ ] Create placeholder buttons for History/Verlauf
- [ ] Create placeholder buttons for Settings/Einstellungen
- [ ] Implement routing between different views
- [ ] Test navigation between all sections

### Story 0.3: System Tray Integration
- [ ] Implement minimize to system tray instead of close
- [ ] Add application icon to system tray
- [ ] Implement double-click to restore window from tray
- [ ] Create right-click context menu for tray icon
- [ ] Add "Exit/Beenden" option in context menu
- [ ] Ensure proper cleanup on exit

### Story 0.4: Dashboard-Layout (Wireframe)
- [ ] Design dashboard layout with logical zones
- [ ] Create placeholder for live status section (top)
- [ ] Create placeholder for history graphs (bottom)
- [ ] Create placeholder for location tag display
- [ ] Implement responsive layout behavior
- [ ] Test window resizing and element adaptation

---

## Epic 1: Netzwerk-Monitoring (Ping & Traffic)

### Story 1.1: Regelmäßiger Ping
- [ ] Implement background ping functionality
- [ ] Configure ping to reliable server (e.g., Google DNS 8.8.8.8)
- [ ] Add configurable ping interval (5s, 10s, 60s options)
- [ ] Store ping response times (ms) in data structure
- [ ] Ensure non-blocking UI during ping operations
- [ ] Add error handling for failed pings
- [ ] Display current latency in UI

### Story 1.2: Traffic-Überwachung
- [ ] Read network adapter statistics (Bytes Sent/Received)
- [ ] Calculate upload/download rates
- [ ] Convert to readable units (KB/s, MB/s)
- [ ] Display real-time traffic in UI
- [ ] Update traffic display at regular intervals
- [ ] Handle multiple network adapters

---

## Epic 2: Standort & Kontext

### Story 2.1: Standort-Tracking
- [ ] Integrate Windows Location API
- [ ] Request location permissions
- [ ] Capture GPS coordinates (latitude/longitude)
- [ ] Store location data with each measurement
- [ ] Implement fallback for unavailable GPS ("Standort unbekannt")
- [ ] Test location accuracy and update frequency

### Story 2.2: Kontext-Kategorisierung (Tagging)
- [ ] Create predefined categories (Zug, Auto, Zuhause, Café, Unterwegs)
- [ ] Implement dropdown/quick select UI for context selection
- [ ] Store selected context with measurements
- [ ] Persist context selection until manually changed
- [ ] Add ability to create custom categories
- [ ] Display current context in UI

---

## Epic 3: Benachrichtigungen & Alerts

### Story 3.1: Warnung bei Verbindungsabbau
- [ ] Define configurable alert thresholds (e.g., Ping > 200ms)
- [ ] Implement Windows Toast Notification system
- [ ] Trigger alert when threshold exceeded
- [ ] Add red icon/visual indicator for poor connection
- [ ] Implement "Snooze" functionality (ignore for X minutes)
- [ ] Add settings UI for threshold configuration
- [ ] Test notification reliability

### Story 3.2: Info bei Wiederherstellung
- [ ] Detect when connection returns to normal range
- [ ] Implement stability check (30s stable before notification)
- [ ] Send recovery notification
- [ ] Add green icon/visual indicator for good connection
- [ ] Prevent notification "flicker" during unstable periods

---

## Epic 4: Datenhistorie & UI

### Story 4.1: Verlaufansicht
- [ ] Choose and integrate charting library
- [ ] Implement line chart for ping times
- [ ] Add red markers for connection failures
- [ ] Implement date/time filtering
- [ ] Add zoom and pan functionality
- [ ] Display historical traffic data
- [ ] Add legend and axis labels

### Story 4.2: Datenexport
- [ ] Create export button in settings
- [ ] Implement CSV export functionality
- [ ] Include all data fields: Timestamp, Ping, Download, Upload, GPS, Context
- [ ] Add file save dialog
- [ ] Test CSV format with Excel/other tools
- [ ] Add date range selection for export

---

## Epic 5: Konfiguration

### Story 5.1: Autostart
- [ ] Create settings UI for autostart option
- [ ] Implement Windows startup registry entry
- [ ] Add "Start with Windows" checkbox
- [ ] Configure app to minimize to tray on startup
- [ ] Test autostart functionality
- [ ] Add option to disable autostart

---

## Additional Technical Tasks

### Data Persistence
- [ ] Choose database solution (SQLite recommended)
- [ ] Design database schema
- [ ] Implement data access layer
- [ ] Add automatic data cleanup (old records)
- [ ] Implement data migration strategy

### Testing & Quality
- [ ] Unit tests for core functionality
- [ ] Integration tests for network monitoring
- [ ] UI automation tests
- [ ] Performance testing
- [ ] Memory leak testing

### Documentation
- [ ] User manual/help documentation
- [ ] Installation guide
- [ ] Developer documentation
- [ ] README file
- [ ] License information

### Deployment
- [ ] Create installer package
- [ ] Code signing certificate
- [ ] Version management
- [ ] Update mechanism
- [ ] Distribution strategy

---

## Notes
- Priority focus: Epic 0 and Epic 1 (Core functionality)
- Consider using async/await for all network operations
- Ensure proper resource cleanup and memory management
- Plan for localization (German/English)
