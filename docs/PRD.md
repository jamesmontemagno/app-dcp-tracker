# DCP Tracker PRD (v1)

Date: 2026-05-15
Status: Draft for review
Owner: Product + Engineering

## 1. Product Summary

DCP Tracker is a mobile-first financial planning app for Microsoft employees that helps users visualize retirement-related accounts (401(k), DCP where eligible, RSU/ESPP), monitor concentration risk, model retirement and vesting scenarios, and generate clear what-if insights.

v1 focus: trusted visibility and planning simulation with manual data entry/import first, then connected data in later phases.

## 2. Problem Statement

Microsoft employees have multiple retirement and wealth components across systems and timelines:
- 401(k) contribution choices and limits
- DCP elections and distribution timing (for eligible users)
- RSU/ESPP vesting and concentration risk
- Retirement timing tradeoffs (55/60/65, leave in 1-3 years)

Today these are fragmented across portals and hard to evaluate together. Users need one place to:
- understand current position,
- see future cash flow and vest/payout events,
- test scenarios without getting prescriptive financial advice.

## 3. Goals and Non-Goals

### Goals
- Provide a unified dashboard for retirement and deferred compensation tracking.
- Enable deep scenario planning (timeline, cash flow, tax-aware estimates, stress cases).
- Show actionable descriptive insights (no direct advisory recommendations in v1).
- Support iOS and Android with secure local-first storage.
- Ship MVP in 6-8 weeks.

### Non-Goals (v1)
- Full direct API connectivity to all providers.
- AI-generated personal financial advice.
- Family/advisor sharing workflows.
- Brokerage execution or transaction initiation.

## 4. Target Users

### Primary
- Microsoft employees (US) tracking 401(k), equity, and retirement progress.

### Secondary
- DCP-eligible Microsoft leaders needing payout/distribution planning.

## 5. Research Findings (Deep-Dive)

## 5.1 Microsoft plan mechanics (high-confidence sources)

Source: Microsoft US Benefits pages
- DCP is described as a non-qualified plan for Microsoft US leaders level 67+.
- DCP deferrals are bookkeeping entries; obligations are unsecured and subject to general creditor risk.
- 401(k) eligibility includes approved regular US payroll employees age 18+ (interns/visiting researchers excluded).
- Contribution options include pre-tax, Roth, after-tax (non-Roth), and catch-up logic tied to SECURE 2.0 updates.
- Investment controls support per-source allocation (pre-tax, Roth, after-tax) and exchange/rebalance in plan-defined increments.

References:
- https://usbenefits.microsoft.com/us/en/deferred-compensation-plan.html
- https://usbenefits.microsoft.com/us/en/401k-plan.html
- https://usbenefits.microsoft.com/us/en/contributionoptions.html
- https://usbenefits.microsoft.com/us/en/investmentoptions.html

## 5.2 Legal and plan-operations constraints (high-confidence sources)

Source: SEC exhibit + 26 USC 409A
- Plan deferral elections are irrevocable once submitted for cycle.
- Distribution schedules and change rules are constrained (timing and anti-acceleration rules).
- Key employees can be subject to a 6-month separation-from-service delay.
- Unforeseeable emergency withdrawals have narrow criteria.
- 409A failures can trigger immediate income inclusion, penalties, and interest.

References:
- https://www.sec.gov/Archives/edgar/data/789019/000119312511200680/dex105.htm
- https://www.law.cornell.edu/uscode/text/26/409A

Note: The SEC plan text captured is a historical restatement and shows different eligibility language (level 68+) than current benefits pages (67+). The app must externalize eligibility rules as configurable and avoid hard-coding.

## 5.3 Contribution-limit context (high-confidence source)

Source: IRS 457(b) contribution limits page
- Annual limit is the lesser of includible compensation or IRS elective deferral limit.
- Catch-up pathways exist with specific rules and cannot always be stacked.

Reference:
- https://www.irs.gov/retirement-plans/plan-participant-employee/retirement-topics-457b-contribution-limits

## 5.4 Data connectivity feasibility (moderate confidence)

Provider research indicates:
- Fidelity WorkplaceXchange APIs exist but are positioned for plan sponsor/advisor/workplace integration, not simple retail consumer self-serve API keys.
- Aggregators (MX, Yodlee) support broad account aggregation and permissioned access patterns, but exact NetBenefits account coverage and reliability should be validated by pilot.

References:
- https://workplacexchange.fidelity.com/public/wpx/api-catalog
- https://www.mx.com/products/account-aggregation/
- https://www.yodlee.com/data-aggregation

Implication: v1 should prioritize manual import + robust data model, then add connectors behind feature flags.

## 6. Product Principles

- Clarity over complexity: users should understand their future cash flow in under 60 seconds.
- Scenario-first UX: every key number should be explorable through what-if analysis.
- Compliance by design: educational and descriptive language, explicit disclaimers.
- Secure by default: encrypted at rest, biometric gating, least-privilege permissions.

## 7. User Stories (v1)

- As a user, I can add and edit accounts for 401(k), DCP, RSU, and ESPP.
- As a user, I can import balances and holdings from CSV or manual entry.
- As a user, I can see total retirement assets, allocation mix, and concentration in employer stock.
- As a user, I can define retirement age assumptions and compare deterministic outcomes.
- As a user, I can model leaving Microsoft in 1-3 years and see impact on vesting and future contributions.
- As a user, I can simulate max 401(k)+DCP paths and market drawdown scenarios.
- As a user, I can view a timeline of upcoming vest, payout, and contribution-limit events.
- As a user, I can receive alerts for contribution drift, concentration risk, key deadlines, and limits.
- As a user, I can read plain-language insights explaining what changed in a scenario.

## 8. Functional Requirements

## 8.1 Accounts and Data
- Support account types: 401(k), DCP, brokerage/other retirement, RSU, ESPP.
- Support assets by source bucket: pre-tax, Roth, after-tax, taxable equity.
- Import methods:
  - CSV upload or pasted delimited rows (provider templates + optional mapping follow-up)
  - manual entry
  - pasted statement text parser
- Maintain data lineage metadata: source type, import time, confidence, user override.

## 8.2 Dashboard
- Combined balance cards and trend.
- Customizable home modules so users can show or hide planning sections.
- Allocation donut/stack by account type and tax bucket.
- Employer stock concentration meter with threshold-based warnings.
- Upcoming events strip (vesting dates, election windows, estimated payout starts).

## 8.3 Planning and Simulation
- Scenario engine with reusable presets:
  - user-defined retirement age
  - leave in 1-3 years
  - max 401(k)+DCP
  - market drawdown (-20%)
- Output views:
  - projected balance over time
  - estimated retirement income/cash flow by year
  - vesting and payout timeline
  - deterministic first-pass estimates with assumptions shown clearly
- Support multiple horizons (1y, 5y, to retirement).

## 8.4 Insights and Alerts
- Rules-based insight generation only (v1).
- Alerts:
  - contribution drift below target
  - concentration risk
  - upcoming vest/payout events
  - annual limit reminders/catch-up windows
- Alerts are in-app only in v1.

## 8.5 Security and Privacy
- On-device encrypted data store.
- Biometric unlock option.
- No cloud sync in v1.
- Explicit consent for any external import/connector.

## 8.6 Compliance and Language
- Educational-only disclaimer in onboarding and insights surfaces.
- Avoid imperative advice text (for example, do not say "you should buy/sell").
- Show source and timestamp for imported values.

## 9. Non-Functional Requirements

- Performance: dashboard render under 1.5s for median dataset.
- Reliability: scenario recompute under 500ms for common models.
- Accessibility: dynamic type, screen reader labels, contrast-compliant charts.
- Offline: full core functionality available offline.

## 10. Data Model (Initial)

Entities:
- UserProfile
- Account
- Position
- ContributionPlan
- VestEvent
- PayoutElection
- Scenario
- ScenarioResult
- AlertRule
- AlertEvent
- ImportJob

Key model notes:
- Keep rule tables versioned (limits, catch-up rules, thresholds).
- Separate factual data from assumptions used by scenario runs.

## 11. Metrics and KPIs

Primary KPI:
- Percentage of active users who complete a full retirement projection.

Supporting KPIs:
- Time to first dashboard completion.
- Weekly active users.
- Percentage of users with at least one scenario saved.
- Alert engagement rate.

## 12. Release Plan (6-8 Week MVP)

Phase 1 (Weeks 1-2): Foundations
- Data schema, local encryption, account CRUD, manual entry.
- Basic dashboard and allocation/concentration visuals.

Phase 2 (Weeks 3-4): Imports + timeline
- CSV import and mapping.
- Vest/payout timeline model and UI.
- Alert engine baseline.

Phase 3 (Weeks 5-6): Scenario engine
- Preset scenarios and comparison views.
- Rules-based narrative insights.

Phase 4 (Weeks 7-8): Hardening
- Accessibility, performance pass, UX polish.
- Compliance copy review.
- Beta feedback and defect fixes.

## 13. Risks and Mitigations

- Risk: Plan rule drift and changing eligibility.
  - Mitigation: externalized rule configuration + source metadata.
- Risk: Data import quality and user trust.
  - Mitigation: import validation, confidence labels, edit history.
- Risk: Misinterpretation as financial advice.
  - Mitigation: strict language linting for insights + disclaimer system.
- Risk: API integration unpredictability with workplace accounts.
  - Mitigation: keep connectors as optional v2 with pilot testing.

## 14. Open Questions for Next Iteration

- What exact DCP election-window reminders are required for current policy year?
- What exact tax bracket presets and withholding explanations should ship after beta feedback?
- What provider-specific mapping templates deserve first-class import support after pasted-row validation?

## 15. Future Roadmap (Post-MVP)

- Provider connections (aggregator + direct where available).
- AI-assisted plain-language summaries with strict compliance guardrails.
- Optional secure cloud backup and multi-device sync.
- Advisor/collaboration workflows and export bundles.
