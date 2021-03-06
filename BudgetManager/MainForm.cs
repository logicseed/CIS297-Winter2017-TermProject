﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BudgetManager
{
    public partial class MainForm : Form
    {
        TransactionQueries transactionQ = new TransactionQueries();

        public MainForm()
        {
            InitializeComponent();
        }

        private void debugDbEditorButton_Click(object sender, EventArgs e)
        {
            var debugDbEditor = new DebugDbEditor();
            debugDbEditor.Show();
        }

        private void mockUpBtn_Click(object sender, EventArgs e)
        {
            var mockUp = new Mockup();
            mockUp.ShowDialog();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'databaseDataSet.Budget' table. You can move, or remove it, as needed.
            this.budgetTableAdapter.Fill(this.databaseDataSet.Budget);
            // TODO: This line of code loads data into the 'databaseDataSet.Account' table. You can move, or remove it, as needed.
            this.accountTableAdapter.Fill(this.databaseDataSet.Account);
            // TODO: This line of code loads data into the 'databaseDataSet.Goal' table. You can move, or remove it, as needed.
            this.goalTableAdapter.Fill(this.databaseDataSet.Goal);
            // TODO: This line of code loads data into the 'databaseDataSet.Transaction' table. You can move, or remove it, as needed.
            this.transactionTableAdapter.Fill(this.databaseDataSet.Transaction);
            RefreshAccountInfo();
            RefreshBudgetInfo();
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            transactionTableAdapter.Fill(databaseDataSet.Transaction);
            transactionGridView.Update();
            transactionGridView.Refresh();
            goalTableAdapter.Fill(databaseDataSet.Goal);
            goalGridView.Update();
            goalGridView.Refresh();
        }

        private void addAccountButton_Click(object sender, EventArgs e)
        {
            var newAccount = databaseDataSet.Account.NewAccountRow();
            newAccount.Name = "New Account";
            newAccount.Type = AccountType.Cash;
            databaseDataSet.Account.Rows.Add(newAccount);
            accountTableAdapter.Update(databaseDataSet.Account);
            var accountID = newAccount.AccountID;
            var position = accountBindingSource.Find("AccountID", accountID);
            accountBindingSource.Position = position;

            var accountForm = new AccountForm(accountID);
            accountForm.ShowDialog();

            accountTableAdapter.Fill(databaseDataSet.Account);
            accountBindingSource.Position = position;
            accountBindingSource.ResetCurrentItem();
        }

        private void editAccountButton_Click(object sender, EventArgs e)
        {
            if (accountBindingSource.Count < 1)
            {
                addAccountButton_Click(sender, e);
                return;
            }
            var accountID = (int)((DataRowView)accountBindingSource.Current).Row["AccountID"];
            var accountForm = new AccountForm(accountID);
            accountForm.ShowDialog();

            accountTableAdapter.Fill(databaseDataSet.Account);
            var position = accountBindingSource.Find("AccountID", accountID);
            accountBindingSource.Position = position;
            accountBindingSource.ResetCurrentItem();
        }

        private void addTransactionButton_Click(object sender, EventArgs e)
        {
            if (accountBindingSource.Count < 1)
            {
                MessageBox.Show("Create an account first.");
                return;
            }
            var newTransaction = databaseDataSet.Transaction.NewTransactionRow();
            newTransaction.AccountID = (int)((DataRowView)accountBindingSource.Current).Row["AccountID"];
            newTransaction.Amount = 0.0f;
            newTransaction.Memo = "No memo";
            newTransaction.Category = TransactionCategory.Uncategorized;
            newTransaction.Date = DateTime.Now;
            databaseDataSet.Transaction.Rows.Add(newTransaction);
            transactionTableAdapter.Update(databaseDataSet.Transaction);
            var transactionID = newTransaction.TransactionID;
            var position = transactionBindingSource.Find("TransactionID", transactionID);
            transactionBindingSource.Position = position;

            var transactionForm = new TransactionForm(newTransaction.TransactionID);
            transactionForm.ShowDialog();

            transactionTableAdapter.Fill(databaseDataSet.Transaction);
            position = transactionBindingSource.Find("TransactionID", transactionID);
            transactionBindingSource.Position = position;
            transactionBindingSource.ResetCurrentItem();
        }

        private void editTransactionButton_Click(object sender, EventArgs e)
        {
            if (transactionBindingSource.Count < 1)
            {
                addTransactionButton_Click(sender, e);
                return;
            }
            var selectedIndex = transactionGridView.SelectedRows[0].Index;
            var transactionID = (int)transactionGridView.CurrentRow.Cells[0].Value;
            var transactionForm = new TransactionForm(transactionID);
            transactionForm.ShowDialog();

            transactionTableAdapter.Fill(databaseDataSet.Transaction);
            var position = transactionBindingSource.Find("TransactionID", transactionID);
            transactionBindingSource.Position = position;
            transactionBindingSource.ResetCurrentItem();
            transactionGridView.ClearSelection();
            transactionGridView.Rows[selectedIndex].Selected = true;
        }

        private void RefreshAccountInfo()
        {
            if (accountBindingSource.Count > 0)
            {
                var accountID = (int)((DataRowView)accountBindingSource.Current).Row["AccountID"];
                transactionBindingSource.RemoveFilter();
                transactionBindingSource.Filter = "AccountID = " + accountID;
                transactionGridView.Refresh();

                var accountBalance = transactionQ.AmountsByAccount(accountID);
                accountBalanceLabel.Text = accountBalance.ToString("C2");
            }
        }

        private void addBudgetButton_Click(object sender, EventArgs e)
        {
            var newBudget = databaseDataSet.Budget.NewBudgetRow();
            newBudget.Name = "New Budget";
            newBudget.StartDate = DateTime.Now;
            newBudget.EndDate = newBudget.StartDate.AddMonths(1);
            databaseDataSet.Budget.Rows.Add(newBudget);
            budgetTableAdapter.Update(databaseDataSet.Budget);
            var budgetID = newBudget.BudgetID;
            var position = budgetBindingSource.Find("BudgetID", budgetID);
            budgetBindingSource.Position = position;

            var budgetForm = new BudgetForm(budgetID);
            budgetForm.ShowDialog();

            budgetTableAdapter.Fill(databaseDataSet.Budget);
            budgetBindingSource.Position = position;
            budgetBindingSource.ResetCurrentItem();
        }

        private void editBudgetButton_Click(object sender, EventArgs e)
        {
            if (budgetBindingSource.Count < 1)
            {
                addBudgetButton_Click(sender, e);
                return;
            }
            var budgetID = (int)((DataRowView)budgetBindingSource.Current).Row["BudgetID"];
            var budgetForm = new BudgetForm(budgetID);
            budgetForm.ShowDialog();

            budgetTableAdapter.Fill(databaseDataSet.Budget);
            var position = budgetBindingSource.Find("BudgetID", budgetID);
            budgetBindingSource.Position = position;
            budgetBindingSource.ResetCurrentItem();
        }

        private void RefreshBudgetInfo()
        {
            if (budgetBindingSource.Count > 0)
            {
                var budgetID = (int)((DataRowView)budgetBindingSource.Current).Row["BudgetID"];
                goalBindingSource.RemoveFilter();
                goalBindingSource.Filter = "BudgetID = " + budgetID;
                goalGridView.Refresh();
                BuildGoalBalanceColumns();
                CalculateBudgetBalance();
            }
        }

        private void addGoalButton_Click(object sender, EventArgs e)
        {
            if (budgetBindingSource.Count < 1)
            {
                MessageBox.Show("Create a budget first.");
                return;
            }

            var newGoal = databaseDataSet.Goal.NewGoalRow();
            newGoal.BudgetID = (int)((DataRowView)budgetBindingSource.Current).Row["BudgetID"];
            newGoal.Amount = 0.0f;
            newGoal.Category = TransactionCategory.Uncategorized;
            databaseDataSet.Goal.Rows.Add(newGoal);
            goalTableAdapter.Update(databaseDataSet.Goal);
            var goalID = newGoal.GoalID;
            var position = goalBindingSource.Find("GoalID", goalID);
            goalBindingSource.Position = position;

            var goalForm = new GoalForm(goalID);
            goalForm.ShowDialog();

            goalTableAdapter.Fill(databaseDataSet.Goal);
            position = goalBindingSource.Find("GoalID", goalID);
            goalBindingSource.Position = position;
            goalBindingSource.ResetCurrentItem();
        }

        private void editGoalButton_Click(object sender, EventArgs e)
        {
            if (goalBindingSource.Count < 1)
            {
                addGoalButton_Click(sender, e);
                return;
            }
            var selectedIndex = goalGridView.SelectedRows[0].Index;
            var goalID = (int)goalGridView.CurrentRow.Cells[0].Value;
            var goalForm = new GoalForm(goalID);
            goalForm.ShowDialog();

            goalTableAdapter.Fill(databaseDataSet.Goal);
            var position = goalBindingSource.Find("GoalID", goalID);
            goalBindingSource.Position = position;
            goalBindingSource.ResetCurrentItem();
            goalGridView.ClearSelection();
            goalGridView.Rows[selectedIndex].Selected = true;
        }

        private void accountBindingSource_CurrentChanged(object sender, EventArgs e)
        {
            RefreshAccountInfo();
        }

        private void budgetBindingSource_CurrentChanged(object sender, EventArgs e)
        {
            RefreshBudgetInfo();
        }

        private void BuildGoalBalanceColumns()
        {
            goalGridView.ReadOnly = false;
            // get start date and end date
            var startDate = (DateTime)((DataRowView)budgetBindingSource.Current).Row["StartDate"];
            var endDate = (DateTime)((DataRowView)budgetBindingSource.Current).Row["EndDate"];
            // loop through rows
            foreach (DataGridViewRow row in goalGridView.Rows)
            {
                // get category
                var category = (TransactionCategory)row.Cells[2].Value;
                // get sum of all transactions during that time period for that category
                var filterString = $"Category = {(int)category} AND Date >= '{startDate}' AND Date <= '{endDate}'";
                DataRow[] transactions = databaseDataSet.Tables["Transaction"].Select(filterString);
                double balance = (double)row.Cells[3].Value;
                foreach (var transaction in transactions)
                {
                    balance += (double)transaction["Amount"];
                }
                // set balance column
                row.Cells[4].Value = balance;
                goalGridView.UpdateCellValue(4, row.Index);
            }
            goalGridView.ReadOnly = true;
            goalGridView.Refresh();
        }

        private void CalculateBudgetBalance()
        {
            var amount = 0.0;
            var balance = 0.0;

            foreach (DataGridViewRow row in goalGridView.Rows)
            {
                amount += (double)row.Cells[3].Value;
                balance += (double)row.Cells[4].Value;
            }

            budgetAmountLabel.Text = amount.ToString("C2");
            budgetBalanceLabel.Text = balance.ToString("C2");
        }

        private void budgetManagerTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshAccountInfo();
            RefreshBudgetInfo();
        }
    }
}
