# Handling Consistency Without Native Transactions in Cosmos DB (Gremlin)

## 📌 Context

This Proof of Concept (PoC) explores how to safely handle **multi-step write operations** in **Azure Cosmos DB (Gremlin API)**, which does **not provide native ACID transactions across multiple vertices and edges**.

In real-world systems, business operations often require:
- Creating multiple vertices
- Creating relationships (edges) between them
- Ensuring that **partial failures do not leave invalid or inconsistent data in the graph**

This PoC demonstrates a **market-standard approach** to deal with this limitation **without attempting to simulate relational transactions**, which is considered an anti-pattern in Cosmos DB.

---

## ❗ The Problem

Consider the following real scenario:

- A `Order` vertex must be created
- Multiple `Item` vertices must be created
- An edge `CONTAINS` must be created between the `Order` and each `Item`

If **any item creation fails**, the entire operation becomes **invalid**.

Challenges:
- Cosmos DB Gremlin does not support multi-entity transactions
- Partial writes may leave **orphan or invalid vertices**
- Immediate rollback (`delete`) is expensive in RU and unsafe
- The system must:
  - Notify the user about the failure
  - Prevent invalid data from being considered “active”
  - Clean up the graph safely and efficiently

---

## 🎯 Goals of This PoC

- Demonstrate how to model **safe write operations without ACID transactions**
- Avoid graph pollution and uncontrolled RU consumption
- Keep the solution aligned with **Clean Architecture**
- Ensure the domain remains **consistent and testable**
- Apply **patterns commonly used in production systems**

---

## 🧠 Architectural Approach

This PoC combines the following **well-established patterns**:

### ✅ Two-Phase Logical Commit
- Entities are first created in a `Pending` state
- Only after all steps succeed, the state transitions to `Active`
- If any step fails, the aggregate transitions to `Failed`

> This is a *logical* commit, not a database-level transaction.

---

### ✅ Aggregate Root Governing Consistency
- `Order` is the **Aggregate Root**
- `Item` entities do not have an independent lifecycle
- If an `Order` is `Failed`, all its items are implicitly invalid

> The state of the aggregate root defines the validity of the entire graph sub-tree.

---

### ✅ Saga (Local, Application-Level)
- The use case orchestrates multiple steps
- Failures are handled explicitly
- Compensation is logical (state change), not transactional rollback

---

### ✅ State Machine for Transitions
Valid transitions:
```text
Pending → Active
Pending → Failed
