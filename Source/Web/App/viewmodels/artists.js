define(['plugins/router', 'services/logger', 'services/dataContext', 'paginationParameters'],
    function (router, logger, dataContext, paginationParameters) {
        var title = 'Artists';
        var vm = {
            activate: activate,
            title: title,
            attached: attached,
            paginationParameters: ko.observable(getDefaultPaginationParameters()),
            pendingRequest: ko.observable(true),
            artists: ko.observableArray([]),
            searchTerm: ko.observable(),
            search: search
        };

        return vm;

        function activate() {
            dataContext.getArtists(vm.paginationParameters)
                .done(function (result) {
                    vm.paginationParameters().totalItemCount(result.Count);
                    vm.artists(result.Items);
                    vm.pendingRequest(false);
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
            
            $('form input.search').typeahead(
                {
                    name: 'artistSearch',
                    remote: '/api/artists/suggest?search=%QUERY'
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

            var query;
            if (vm.searchTerm() && vm.searchTerm().length > 0)
                query = dataContext.searchArtists(vm.searchTerm(), vm.paginationParameters);
            else
                query = dataContext.getArtists(vm.paginationParameters);

            query.done(function (result) {
                for (var i = 0; i < result.Items.length; i++) {
                    vm.artists.push(result.Items[i]);
                }

                vm.paginationParameters().pendingScrollRequest(false);
            });
        }
        
        function search() {
            if (vm.searchTerm().length === 0)
                return false;

            var params = getDefaultPaginationParameters();
            params.sortCommand(null);
            vm.paginationParameters(params);
            searchWithParameters(vm.paginationParameters);
        }
        
        function searchWithParameters(parameters) {
            vm.pendingRequest(true);
            dataContext.searchArtists(vm.searchTerm(), parameters)
                .done(function (result) {
                    vm.paginationParameters().totalItemCount(result.Count);
                    vm.artists([]);
                    for (var i = 0; i < result.Items.length; i++) {
                        vm.artists.push(result.Items[i]);
                    }

                    vm.pendingRequest(false);
                });
        }
    });