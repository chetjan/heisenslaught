using Heisenslaught.Models.Draft;
using Heisenslaught.Models.Users;
using Heisenslaught.Persistence.User;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

namespace Heisenslaught.Persistence.Draft
{
    public class DraftListViewGenerator
    {
        private IDraftStore draftStore;
        private IUserStore<HSUser> userStore;
        private DraftListViewStore viewStore;
        private UserManager<HSUser> userManager;

        public DraftListViewGenerator(IDraftStore draftStore, IUserStore<HSUser> userStore, DraftListViewStore viewStore, UserManager<HSUser> userManager)
        {
            this.draftStore = draftStore;
            this.userStore = userStore;
            this.viewStore = viewStore;
            this.userManager = userManager;

            if (viewStore.ShouldInitializeGenerators)
            {
                RegenerateAll();
            }

            draftStore.OnCreated += OnDraftCreatedAsync;
            draftStore.OnUpdated += OnDraftUpdatedAsync;
            draftStore.OnDeleted += OnDraftDeleted;

            if (userStore is HSUserStore)
            {
              // not needed yet
                //  ((HSUserStore)userStore).OnUpdated += OnUserUpdated;
            }
        }

        private async void OnDraftCreatedAsync(object sender, DraftModel draft)
        {
            var user = draft.createdBy != null ? await userManager.FindByIdAsync(draft.createdBy) : null;
            CreateViewRecord(draft, user);
        }

        private async void OnDraftUpdatedAsync(object sender, DraftModel draft)
        {
            var user = draft.createdBy != null ? await userManager.FindByIdAsync(draft.createdBy) : null;
            UpdateViewRecord(draft, user);
        }

        private void OnDraftDeleted(object sender, DraftModel draft)
        {
            DeleteViewRecord(draft);
        }

        private void OnUserUpdated(object sender, HSUser user)
        {
            var drafts = draftStore.FindByUserId(user.Id);
            foreach(var draft in drafts)
            {
                UpdateViewRecord(draft, user);
            }
        }

        private DraftListViewModel MakeView(DraftModel model, HSUser user)
        {
            return new DraftListViewModel()
            {
                Id = model.Id,
                userId = user?.Id,
                battleTag = user?.BattleTag,
                team1Name = model.config.team1Name,
                team2Name = model.config.team2Name,
                draftToken = model.draftToken,
                adminToken = model.adminToken,
                team1DrafterToken = model.team1DrafterToken,
                team2DrafterToken = model.team2DrafterToken,
                map = model.config.map,
                phase = (int)model.state.phase
            };
        }

        private void CreateViewRecord(DraftModel model, HSUser user)
        {
            viewStore.Create(MakeView(model, user));
        }

        private void UpdateViewRecord(DraftModel model, HSUser user)
        {
            viewStore.Update(MakeView(model, user));
        }

        private void DeleteViewRecord(DraftModel model)
        {
            viewStore.Delete(new DraftListViewModel() { Id = model.Id });
        }

        public void RegenerateAll()
        {
            viewStore.Collection.DeleteMany(Builders<DraftListViewModel>.Filter.Empty);
            var drafts = draftStore.FindAll();
            foreach(var draft in drafts)
            {
                OnDraftCreatedAsync(this, draft);
            }
        }


    }
}
