using Essence.Comms;
using Essence.Dto;
using Essence.Global;
using Essence.Injection;
using Essence.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xciles.PclValueInjecter;

namespace Essence.ViewModel
{
    public class SearchCollection : ObservableCollection<SearchModel>
    {
        public void CopyFrom(SearchDTO SearchDTO)
        {
            this.Items.Clear();
            SearchModel SearchModel = new SearchModel();
            SearchModel.InjectFrom(SearchDTO);
            SearchModel.Search = new List<SerieModel>();
            foreach (var serie in SearchDTO.Search)
            {
                SerieModel SerieModel = new SerieModel();
                SerieModel.InjectFrom(serie);
                SearchModel.Search.Add(SerieModel);
            }
            this.Items.Add(SearchModel);
            this.OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public async Task Get(IWindow EssenceWindows, string Title)
        {
            using (var SearchRepository = new SearchRepository())
            {
                try
                {
                    EssenceWindows.ShowWaitIndicator = true;
                    SearchDTO response = await SearchRepository.GetSearchAsyncByTitle(Title);
                    this.CopyFrom(response);
                    EssenceWindows.NotifyChange(Essence.Enums.ModelsEnum.TiposModels.Search);
                }
                catch (HttpRequestException ex)
                {
                    EssenceWindows.ShowError(String.Format("{0}\n{1}", ex.Message, SearchRepository.ResponseError()));
                }
                catch (System.FormatException)
                {
                    EssenceWindows.ShowError(String.Format("{0}", "Error al recuperar Search"));
                }
                finally
                {
                    EssenceWindows.ShowWaitIndicator = false;
                }
            }
            return;
        }



    }
}
