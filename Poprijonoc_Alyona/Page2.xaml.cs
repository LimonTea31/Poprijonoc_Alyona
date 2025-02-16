﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Poprijonoc_Alyona
{
    /// <summary>
    /// Логика взаимодействия для Page2.xaml
    /// </summary>
    public partial class Page2 : Page
    {
        Agent agent;
        int curSelPr;
        public Page2(Agent ag)
        {
            InitializeComponent();
            try
            {
                Type.ItemsSource = helper.GetContext().AgentType.ToList();
                product.ItemsSource = helper.GetContext().Product.ToList();
            }
            catch { };
            if (ag != null)
            {
                agent = ag;
                Type.SelectedItem = ag.AgentType;
                this.Title.Text = ag.Title;
                this.Adress.Text = ag.Address;
                this.Inn.Text = ag.INN;
                this.Kpp.Text = ag.KPP;
                this.DirectorName.Text = ag.DirectorName;
                this.DirectorSurname.Text = ag.DirectorSurname;
                this.DirectorPatronimyc.Text = ag.DirectorPatrymic;
                this.Phone.Text = ag.Phone;
                this.Prioritet.Text = ag.Priority.ToString();
                this.Email.Text = ag.Email.ToString();
                this.Logo.Text = ag.Logo.ToString();
                historyGrid.ItemsSource = helper.GetContext().ProductSale.Where(ProductSale => ProductSale.AgentID == ag.ID).ToList();
            }
            else
            {
                agent = new Agent();
                btnDelAg.IsEnabled = false;
                btnWritHi.IsEnabled = false;
                btnDelHi.IsEnabled = false;
            }
            this.DataContext = agent;
        }

        private void mask_TextChanged(object sender, TextChangedEventArgs e)
        {
            string fnd = ((TextBox)sender).Text;
            try
            {
                product.ItemsSource = helper.GetContext().Product.Where(Product => Product.Title.Contains(fnd)).ToList();
            }
            catch { };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int curTypAg = Type.SelectedIndex + 1;
            if (curTypAg == 0)
            {
                MessageBox.Show("Type не подходит");
                return;
            }
            if (this.Title.Text == "")
            {
                MessageBox.Show("Title не подходит");
                return;
            }
            if (!(new Regex(@"\d{10}|\d{12}")).IsMatch(this.Inn.Text))
            {
                MessageBox.Show("INN не подходит");
                return;
            }
            if (!(new Regex(@"\d{4}[\dA-Z][\dA-Z]\d{3}")).IsMatch(this.Kpp.Text))
            {
                MessageBox.Show("KPP не подходит");
                return;
            }
            if (!(new Regex(@"^\+?\d{0,2}\-?\d{3}\-?\d{3}\-?\d{4}")).IsMatch(this.Phone.Text))
            {
                MessageBox.Show("PHONE не подходит");
                return;
            }
            if ((this.Email.Text != "") && (!(new Regex(@"(\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)")).IsMatch(this.Email.Text)))
            {
                MessageBox.Show("Email не подходит");
                return;
            }
            agent.Title = this.Title.Text;
            agent.AgentTypeID = curTypAg;
            agent.Address = this.Adress.Text;
            agent.INN = this.Inn.Text;
            agent.KPP = this.Kpp.Text;
            agent.Phone = this.Phone.Text;
            agent.DirectorName = this.DirectorName.Text;
            agent.DirectorSurname = this.DirectorSurname.Text;
            agent.DirectorPatrymic = this.DirectorPatronimyc.Text;
            agent.Phone = this.Phone.Text;
            agent.Email = this.Email.Text;
            try
            {
                agent.Priority = Convert.ToInt32(this.Prioritet.Text);
            }
            catch
            {
                return;
            }

            if (agent.ID > 0)
            {
                helper.GetContext().Entry(agent).State = EntityState.Modified;
                helper.GetContext().SaveChanges();
                MessageBox.Show("Обновление информации об агенте завершено");
            }
            else
            {
                helper.ent.Agent.Add(agent);
                helper.ent.SaveChanges();
                MessageBox.Show("Добавление информации об агенте завершено");
            }

            btnDelAg.IsEnabled = true;
            btnWritHi.IsEnabled = true;
            btnDelHi.IsEnabled = true;
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (agent.ProductSale.Count > 0)
            {
                MessageBox.Show("Удаление не возможно!");
                return;
            }
            foreach (Shop shop in agent.Shop)
            {
                helper.GetContext().Shop.Remove(shop);
            }
            foreach (AgentPriorityHistory apr in agent.AgentPriorityHistory)
            {
                helper.GetContext().AgentPriorityHistory.Remove(apr);
            }
            helper.GetContext().Agent.Remove(agent);
            helper.GetContext().SaveChanges();
            MessageBox.Show("Удаление информации об агенте завешено!");
            this.NavigationService.GoBack();
        }

        private void Type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            int cnt = 0;
            try
            {
                cnt = Convert.ToInt32(count.Text);
            }
            catch
            {
                return;
            }
            string dt = date.ToString();
            if (curSelPr > 0 && dt != "" && cnt > 0)
            {
                ProductSale pr = new ProductSale();
                pr.AgentID = agent.ID;
                pr.ProductID = curSelPr;
                pr.SaleDate = (DateTime)date.SelectedDate;
                pr.ProductCount = cnt;
                try
                {
                    helper.GetContext().ProductSale.Add(pr);
                    helper.GetContext().SaveChanges();
                    historyGrid.ItemsSource = helper.GetContext().ProductSale.Where(ProductSale => ProductSale.AgentID == agent.ID).ToList();
                }
                catch
                {
                    return;
                }
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < historyGrid.SelectedItems.Count; i++)
            {
                ProductSale prs = historyGrid.SelectedItems[i] as ProductSale;
                if (prs != null)
                {
                    helper.GetContext().ProductSale.Remove(prs);
                }
            }
            try
            {
                helper.GetContext().SaveChanges();
                historyGrid.ItemsSource = helper.GetContext().ProductSale.Where(ProductSale => ProductSale.AgentID == agent.ID).ToList();
            }
            catch { return; };
        }

        private void historyGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void product_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            curSelPr = ((Product)product.SelectedItem).ID;
        }
    }
}
