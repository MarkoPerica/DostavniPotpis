using CommunityToolkit.Mvvm.ComponentModel;
using DostavniPotpis.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DostavniPotpis.ViewModels
{
    public class TransferViewModel : ObservableObject, INotifyPropertyChanged
    {
        private readonly IApiService _apiService;        
    }
}
