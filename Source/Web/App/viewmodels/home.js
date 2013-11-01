define(['plugins/router', 'services/logger', 'services/dataContext', 'paginationParameters', 'model/discWithImage'],
    function (router, logger, dataContext, paginationParameters, discWithImage) {
        var title = 'Discs';
        var vm = {
            activate: activate,
            title: title,
            attached: attached,
            paginationParameters: ko.observable(getDefaultPaginationParameters()),
            pendingRequest: ko.observable(true),
            discs: ko.observableArray([]),
            sort: sort,
            searchTerm: ko.observable(),
            search: search
        };

        return vm;

        function activate() {
            //todo: use current page to load previous data
            vm.paginationParameters().currentPage(1);
            getDiscs();
            logger.log(title + ' View Activated', null, title, true);
            return true;
        }

        function attached() {
            $(window).scroll(function() {
                if (router.activeItem().title === vm.title && $(window).scrollTop() == $(document).height() - $(window).height()) {
                    loadMoreDiscs();
                }
            });
        }

        function sort(sortBy) {
            if (vm.paginationParameters().sortCommand() === sortBy) {
                //just toggle asc/desc
                var current = vm.paginationParameters().sortDescending();
                vm.paginationParameters().sortDescending(!current);
            } else {
                vm.paginationParameters(getDefaultPaginationParameters(sortBy));
            }

            getDiscs();
        }

        function getDefaultPaginationParameters(sortCommand) {
            sortCommand = sortCommand || 'Released';
            var params = new paginationParameters();
            params.sortCommand(sortCommand);
            params.sortDescending(sortCommand === 'Released' ? true : false);

            return params;
        }

        function loadMoreDiscs() {
            if (vm.paginationParameters().pendingScrollRequest() || !vm.paginationParameters().hasNextPage()) {
                return;
            }

            vm.paginationParameters().pendingScrollRequest(true);

            var nextPage = vm.paginationParameters().currentPage() + 1;
            vm.paginationParameters().currentPage(nextPage);

            var query;
            if (vm.searchTerm() && vm.searchTerm().length > 0)
                query = dataContext.searchDiscs(vm.searchTerm(), vm.paginationParameters);
            else
                query = dataContext.getDiscs(vm.paginationParameters);

            query.done(function(result) {
                for (var i = 0; i < result.Items.length; i++) {
                    vm.discs.push(new discWithImage(result.Items[i]));
                }

                vm.paginationParameters().pendingScrollRequest(false);
            });
        }

        function search() {
            if (vm.searchTerm().length === 0)
                return false;

            vm.paginationParameters(getDefaultPaginationParameters());
            vm.pendingRequest(true);
            dataContext.searchDiscs(vm.searchTerm(), vm.paginationParameters)
                .done(function(result) {
                    vm.paginationParameters().totalItemCount(result.Count);
                    vm.discs([]);
                    for (var i = 0; i < result.Items.length; i++) {
                        vm.discs.push(new discWithImage(result.Items[i]));
                    }

                    vm.pendingRequest(false);
                });
        }

        function getDiscs() {
            dataContext.getDiscs(vm.paginationParameters)
                .done(function(result) {
                    vm.paginationParameters().totalItemCount(result.Count);
                    ko.mapping.fromJS(result.Items, {
                        create: function(options) {
                            return new discWithImage(options.data);
                        }
                    }, vm.discs);
                    vm.pendingRequest(false);
                });
        }
    });