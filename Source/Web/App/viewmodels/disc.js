define(['services/logger', 'services/dataContext', 'model/discWithImage'],
    function (logger, dataContext, discWithImage) {
        var title = 'Disc';
        var vm = {
            activate: activate,
            title: title,
            disc: ko.observable()
        };

        return vm;

        function activate(id) {
            dataContext.getDisc(id)
                .done(function (result) {
                    vm.disc(new discWithImage(result));
                });
            logger.log(title + ' View Activated For ' + id, null, title, true);
            return true;
        }
    });