define(
    function() {
        var paginationParameters = function(onPageChange) {
            var self = this;

            self.sortCommand = ko.observable('Id');
            self.sortDescending = ko.observable(false);
            self.currentPage = ko.observable(1);
            self.totalItemCount = ko.observable(0);
            self.pageSize = ko.observable(25);
            self.pageSlide = ko.observable(5);
            self.pendingScrollRequest = ko.observable(false);

            self.currentPage.subscribe(function () {
                if (onPageChange)
                    onPageChange();
            });

            self.lastPage = ko.computed(function() {
                return Math.floor((self.totalItemCount() - 1) / self.pageSize()) + 1;
            });

            self.hasNextPage = ko.computed(function() {
                return self.currentPage() < self.lastPage();
            });

            self.hasPrevPage = ko.computed(function() {
                return self.currentPage() > 1;
            });

            self.firstItemIndex = ko.computed(function() {
                return self.pageSize() * (self.currentPage() - 1) + 1;
            });

            self.lastItemIndex = ko.computed(function() {
                return Math.min(self.firstItemIndex() + self.pageSize() - 1, self.totalItemCount());
            });

            self.pages = ko.computed(function() {
                var pageCount = self.lastPage();
                var pageFrom = Math.max(1, self.currentPage() - self.pageSlide());
                var pageTo = Math.min(pageCount, self.currentPage() + self.pageSlide());
                pageFrom = Math.max(1, Math.min(pageTo - 2 * self.pageSlide(), pageFrom));
                pageTo = Math.min(pageCount, Math.max(pageFrom + 2 * self.pageSlide(), pageTo));

                var result = [];
                for (var i = pageFrom; i <= pageTo; i++) {
                    result.push(i);
                }
                return result;
            });
        };

        return paginationParameters;
    });