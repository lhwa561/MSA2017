using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using MSA2017_lhwa561.Model;
using System.Diagnostics;

namespace MSA2017_lhwa561
{
    public class AzureManager
    {
        private static AzureManager instance;
        private MobileServiceClient client;
        private IMobileServiceTable<MSA2017lhwa561Table> MSA2017Table;

        private AzureManager()
        {
            this.client = new MobileServiceClient("http://msa2017lhwa561.azurewebsites.net");
            this.MSA2017Table = this.client.GetTable<MSA2017lhwa561Table>();
        }

        public MobileServiceClient AzureClient
        {
            get { return client; }
        }

        public static AzureManager AzureManagerInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AzureManager();
                }

                return instance;
            }
        }

        public async Task<List<MSA2017lhwa561Table>> GetFaceInformation()
        {
            Debug.WriteLine("GET INFO");
            return await this.MSA2017Table.ToListAsync();
        }

        public async Task PostFaceInformation(MSA2017lhwa561Table MSATable)
        {
            await this.MSA2017Table.InsertAsync(MSATable);
        }
    }
}
