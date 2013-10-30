define(['services/logger', 'services/dataContext', 'model/artist'],
    function(logger, dataContext, artist) {
        var title = 'Artist';
        var vm = {
            activate: activate,
            title: title,
            artist: ko.observable()
        };

        return vm;

        function activate(id) {
            dataContext.getArtist(id)
                .done(function(result) {
                    vm.artist(new artist(result));
                });
            logger.log(title + ' View Activated For ' + id, null, title, true);
            return true;
        }
    });