define(['plugins/router', 'services/logger', 'services/dataContext', 'paginationParameters'],
    function (router, logger, dataContext, paginationParameters) {
        var title = 'Artists';
        var vm = {
            activate: activate,
            title: title,
            attached: attached,
            paginationParameters: ko.observable(getDefaultPaginationParameters()),
            artists: ko.observableArray([])
        };

        return vm;

        function activate() {
            dataContext.getArtists(vm.paginationParameters)
                .done(function (result) {
                    vm.paginationParameters().totalItemCount(result.Count);
                    vm.artists(result.Items);
                });
            logger.log(title + ' View Activated', null, title, true);
            return true;
        }
        
        function attached() {
            $(window).scroll(function () {
                if (router.activeItem().title === vm.title && $(window).scrollTop() == $(document).height() - $(window).height()) {
                    loadMoreArtists();
                }
            });
        }

        function getDefaultPaginationParameters() {
            var params = new paginationParameters();
            params.sortCommand('Name');

            return params;
        }
        
        function loadMoreArtists() {
            if (vm.paginationParameters().pendingScrollRequest() || !vm.paginationParameters().hasNextPage()) {
                return;
            }

            vm.paginationParameters().pendingScrollRequest(true);

            var nextPage = vm.paginationParameters().currentPage() + 1;
            vm.paginationParameters().currentPage(nextPage);

            dataContext.getArtists(vm.paginationParameters)
                .done(function (result) {
                    for (var i = 0; i < result.Items.length; i++) {
                        vm.artists.push(result.Items[i]);
                    }

                    vm.paginationParameters().pendingScrollRequest(false);
                });
        }
    });