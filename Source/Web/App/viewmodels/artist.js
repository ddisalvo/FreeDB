define(['services/logger', 'services/dataContext', 'model/artist'],
    function(logger, dataContext, artist) {
        var title = 'Artist';
        var vm = {
            activate: activate,
            title: title,
            artist: ko.observable(),
            pendingRequest: ko.observable(true)
        };

        return vm;

        function activate(id) {
            vm.pendingRequest(true);
            dataContext.getArtist(id)
                .done(function(result) {
                    vm.artist(new artist(result));
                    vm.pendingRequest(false);
                });
            logger.log(title + ' View Activated For ' + id, null, title, true);
            return true;
        }
    });