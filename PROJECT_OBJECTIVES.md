# Stamps - Digital Loyalty Card System

## Project Objective

Create a comprehensive digital stamp/loyalty card application that modernizes the traditional paper-based stamp card system used by stores to reward repeat customers.

## Problem Statement

Traditional paper stamp cards have several limitations:
- Easy to lose or damage
- Limited to physical presence
- No analytics for store owners
- Manual tracking and redemption
- No centralized system across multiple stores

## Solution

A Blazor Server web application that provides:
- Digital stamp cards accessible from any device
- QR code-based stamp collection and redemption
- Real-time synchronization across devices
- Comprehensive analytics for store owners
- Secure, cloud-based storage

## Target Users

### 1. **Customers (Clients)**
- Collect digital stamps from subscribed stores
- View all stamp cards in one place
- Track purchase history
- Redeem rewards via QR codes
- Access from phone, tablet, or desktop

### 2. **Store Owners**
- Register and manage multiple stores
- Scan customer QR codes to add stamps
- Configure stamp requirements per card
- Set custom stamp values per transaction
- View detailed business statistics
- Track customer engagement

## Core Features

### Customer Features
1. **Account Management**
   - Email or Google OAuth registration
   - Separate client account type
   - Secure authentication

2. **Stamp Card Management**
   - View all active stamp cards
   - Multiple cards per store support
   - Real-time stamp balance updates
   - Progress indicators

3. **QR Code System**
   - Generate time-limited QR codes for receiving stamps
   - Generate redemption QR codes when eligible
   - 5-minute token expiration for security

4. **Purchase History**
   - Complete transaction log
   - View stamps added and redeemed
   - Date/time stamps
   - Store information

### Store Owner Features
1. **Store Management**
   - Register multiple stores
   - Configure store information
   - Manage stamp card programs

2. **QR Code Scanning**
   - Scan customer QR codes via web camera
   - Manual QR code entry fallback
   - Add stamps to customer cards
   - Process reward redemptions

3. **Stamp Configuration**
   - Set required stamps for rewards
   - Adjust stamp values per transaction
   - Create multiple card types per store

4. **Business Analytics**
   - Total customer count
   - Active vs completed cards
   - Total stamps issued
   - Redemption statistics
   - Daily activity trends (30-day view)
   - New customer acquisition tracking

## Technical Architecture

### Frontend
- **Blazor Server**: Real-time, server-side rendering
- **Bootstrap 5**: Responsive, mobile-first UI
- **Bootstrap Icons**: Consistent iconography

### Backend
- **.NET 10.0**: Latest framework features
- **ASP.NET Core Identity**: User authentication
- **Entity Framework Core**: Data access
- **PostgreSQL (Supabase)**: Cloud database

### Security
- **QR Token System**: Time-limited, single-use tokens
- **Role-based access**: Separate client/store owner permissions
- **Password requirements**: Strong password enforcement
- **SSL/TLS**: Encrypted connections
- **OAuth Support**: Google authentication

### Infrastructure
- **Database**: Supabase PostgreSQL (free tier)
- **Connection**: Transaction pooler for reliability
- **Mobile Access**: Network-accessible for phone debugging
- **API Support**: RESTful design for future mobile apps

## Business Logic

### Stamp Collection Flow
1. Customer opens app and generates QR code
2. Store owner scans QR code
3. Store owner selects stamp card and quantity
4. Stamps added to customer's card
5. Transaction recorded in history

### Redemption Flow
1. Customer views stamp card showing eligibility
2. Customer clicks "Redeem Reward"
3. System generates redemption QR code
4. Store owner scans redemption QR
5. Required stamps deducted from card
6. Customer receives reward (physical item)

### Stamp Card Rules
- **Multiple Cards**: Stores can offer different card types
- **Flexible Requirements**: Each card has configurable stamp threshold
- **Stamp Deduction**: Redemption deducts required stamps (doesn't reset)
- **Persistence**: Remaining stamps stay on card
- **No Expiration**: Stamps don't expire (store owner policy)

## Privacy & Data Protection

### Customer Privacy
- Store owners cannot view individual customer details
- Only aggregate statistics visible to stores
- Anonymous transaction tracking
- Customer controls their own data

### Data Security
- Encrypted database connections
- Secure password storage (hashed)
- Time-limited QR tokens prevent replay attacks
- Single-use tokens prevent double-redemption

## Scalability Considerations

### Current Implementation
- Supports unlimited customers
- Supports unlimited stores per owner
- Supports unlimited cards per store
- Handles real-time concurrent transactions

### Future Enhancements
- Push notifications for stamp additions
- Loyalty program analytics
- Customer engagement campaigns
- Multi-language support
- Store location mapping
- Social sharing features
- Referral programs

## Success Metrics

### For Customers
- Reduced lost stamp cards
- Centralized card management
- Easy reward redemption
- Transaction transparency

### For Store Owners
- Increased customer retention
- Data-driven business decisions
- Reduced paper waste
- Streamlined redemption process
- Customer behavior insights

## Deployment Strategy

### Development Phase (Current)
- Local development with Supabase
- Mobile debugging via local network
- Manual testing and iteration

### Production Phase (Future)
- Azure App Service or similar hosting
- Custom domain with HTTPS
- Production database with backups
- Monitoring and logging
- Regular security updates

## Technical Specifications

### Database Schema
- **Users**: Authentication and profile data
- **Stores**: Store information and ownership
- **StampCards**: Customer-store card relationships
- **Transactions**: Complete audit trail
- **QRTokens**: Temporary tokens for operations

### API Endpoints
- Account management (login, register, logout)
- Stamp operations (add, redeem)
- Card management (view, create)
- Statistics retrieval
- QR token generation and validation

### Mobile Compatibility
- Responsive web design
- Touch-friendly interface
- Network-accessible server
- QR code camera integration ready
- Progressive Web App (PWA) potential

## Development Timeline

### Phase 1: Foundation ✅
- Database design and setup
- Authentication system
- Basic page structure

### Phase 2: Core Features (In Progress)
- QR code generation and scanning
- Stamp collection workflow
- Redemption system
- Dashboard interfaces

### Phase 3: Enhancement
- Statistics and analytics
- Mobile optimization
- Performance tuning
- User testing

### Phase 4: Production
- Security hardening
- Deployment setup
- Documentation
- Marketing materials

## Maintenance & Support

### Regular Updates
- Security patches
- Bug fixes
- Feature enhancements
- Performance optimization

### Monitoring
- Database health checks
- Error logging
- Usage analytics
- Performance metrics

## License & Usage

This is a custom-built application designed for general use by any store wanting to implement a digital loyalty program.

---

**Project Status**: Active Development  
**Version**: 1.0.0-beta  
**Last Updated**: November 24, 2025  
**Developer**: Custom Build  
**Database**: Supabase PostgreSQL (Free Tier)  
**Platform**: Cross-platform web application

