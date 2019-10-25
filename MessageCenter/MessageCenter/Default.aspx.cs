using MessageCenter.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MessageCenter
{
    public partial class _Default : Page
    {
        public List<MessageTemplate> listOfMessages;

        protected void Page_Load(object sender, EventArgs e)
        {
            PopulateMessagePrefabListBox();
        }

        private void PopulateMessagePrefabListBox()
        {
            listOfMessages = DatabaseManager.Instance.GetMessageTemplates();

            if (listBoxMessageTemplates.Items.Count > 0)
            {
                listBoxMessageTemplates.Items.Clear();
            }


            foreach (MessageTemplate item in listOfMessages)
            {
                System.Diagnostics.Debug.WriteLine(item.title);
                listBoxMessageTemplates.Items.Add(item.title);


            }

            // listBoxMessageTemplates.DataSource = listOfMessages;
        }
    }
}