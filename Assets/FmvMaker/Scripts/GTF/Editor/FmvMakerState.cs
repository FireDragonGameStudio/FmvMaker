using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEngine;
using UnityEngine.GraphToolsFoundation.CommandStateObserver;

namespace FmvMaker.Core.GTF {
    public class FmvMakerState : GraphToolState {
        /// <inheritdoc />
        public FmvMakerState(Hash128 graphViewEditorWindowGUID, Preferences preferences)
            : base(graphViewEditorWindowGUID, preferences) {
            this.SetInitialSearcherSize(SearcherService.Usage.k_CreateNode, new Vector2(375, 300), 2.0f);
        }

        /// <inheritdoc />
        public override void RegisterCommandHandlers(Dispatcher dispatcher) {
            base.RegisterCommandHandlers(dispatcher);

            if (!(dispatcher is CommandDispatcher commandDispatcher))
                return;

            commandDispatcher.RegisterCommandHandler<AddPortCommand>(AddPortCommand.DefaultHandler);
            commandDispatcher.RegisterCommandHandler<RemovePortCommand>(RemovePortCommand.DefaultHandler);

            commandDispatcher.RegisterCommandHandler<SetNameCommand>(SetNameCommand.DefaultHandler);
            commandDispatcher.RegisterCommandHandler<SetDescriptionCommand>(SetDescriptionCommand.DefaultHandler);
            commandDispatcher.RegisterCommandHandler<SetPickUpVideoCommand>(SetPickUpVideoCommand.DefaultHandler);
            commandDispatcher.RegisterCommandHandler<SetIsNavigationCommand>(SetIsNavigationCommand.DefaultHandler);
        }
    }
}