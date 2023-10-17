(function () {

	'use strict';

	var mobileMenuOutsideClick = function () {

		$(document).click(function (e) {
			var container = $("#sc-offcanvas, .js-sc-nav-toggle");
			if (!container.is(e.target) && container.has(e.target).length === 0) {
				$('.js-sc-nav-toggle').addClass('sc-nav-white');

				if ($('body').hasClass('offcanvas')) {

					$('body').removeClass('offcanvas');
					$('.js-sc-nav-toggle').removeClass('active');

				}


			}
		});

	};

	var offcanvasMenu = function () {

		$('#page').prepend('<div id="sc-offcanvas" />');
		$('#page').prepend('<a href="#" class="js-sc-nav-toggle sc-nav-toggle sc-nav-white"><i></i></a>');
		var clone1 = $('.menu-1 > ul').clone();
		$('#sc-offcanvas').append(clone1);
		var clone2 = $('.menu-2 > ul').clone();
		$('#sc-offcanvas').append(clone2);

		$('#sc-offcanvas .has-dropdown').addClass('offcanvas-has-dropdown');
		$('#sc-offcanvas')
			.find('li')
			.removeClass('has-dropdown');

		// Hover dropdown menu on mobile
		$('.offcanvas-has-dropdown').mouseenter(function () {
			var $this = $(this);

			$this
				.addClass('active')
				.find('ul')
				.slideDown(500, 'easeOutExpo');
		}).mouseleave(function () {

			var $this = $(this);
			$this
				.removeClass('active')
				.find('ul')
				.slideUp(500, 'easeOutExpo');
		});


		$(window).resize(function () {

			if ($('body').hasClass('offcanvas')) {

				$('body').removeClass('offcanvas');
				$('.js-sc-nav-toggle').removeClass('active');

			}
		});
	};


	var burgerMenu = function () {

		$('body').on('click', '.js-sc-nav-toggle', function (event) {
			var $this = $(this);


			if ($('body').hasClass('overflow offcanvas')) {
				$('body').removeClass('overflow offcanvas');
			} else {
				$('body').addClass('overflow offcanvas');
			}
			$this.toggleClass('active');
			event.preventDefault();

		});
	};



	var contentWayPoint = function () {
		var i = 0;

		// $('.sc-section').waypoint( function( direction ) {


		$('.animate-box').waypoint(function (direction) {

			if (direction === 'down' && !$(this.element).hasClass('animated-fast')) {

				i++;

				$(this.element).addClass('item-animate');
				setTimeout(function () {

					$('body .animate-box.item-animate').each(function (k) {
						var el = $(this);
						setTimeout(function () {
							var effect = el.data('animate-effect');
							if (effect === 'fadeIn') {
								el.addClass('fadeIn animated-fast');
							} else if (effect === 'fadeInLeft') {
								el.addClass('fadeInLeft animated-fast');
							} else if (effect === 'fadeInRight') {
								el.addClass('fadeInRight animated-fast');
							} else {
								el.addClass('fadeInUp animated-fast');
							}

							el.removeClass('item-animate');
						}, k * 200, 'easeInOutExpo');
					});

				}, 100);

			}

		}, {
			offset: '85%'
		});
		// }, { offset: '90%'} );
	};


	var dropdown = function () {

		$('.has-dropdown').mouseenter(function () {

			var $this = $(this);
			$this
				.find('.dropdown')
				.css('display', 'block')
				.addClass('animated-fast fadeInUpMenu');

		}).mouseleave(function () {
			var $this = $(this);

			$this
				.find('.dropdown')
				.css('display', 'none')
				.removeClass('animated-fast fadeInUpMenu');
		});

	};

	var tabs = function () {
		// Auto adjust height
		$('.sc-tab-content-wrap').css('height', 0);
		var autoHeight = function () {
			setTimeout(function () {
				var tabContentWrap = $('.sc-tab-content-wrap'),
					tabHeight = $('.sc-tab-nav').outerHeight(),
					formActiveHeight = $('.tab-content.active').outerHeight(),
					totalHeight = parseInt(tabHeight + formActiveHeight + 90);
				tabContentWrap.css('height', totalHeight);
				$(window).resize(function () {
					var tabContentWrap = $('.sc-tab-content-wrap'),
						tabHeight = $('.sc-tab-nav').outerHeight(),
						formActiveHeight = $('.tab-content.active').outerHeight(),
						totalHeight = parseInt(tabHeight + formActiveHeight + 90);
					tabContentWrap.css('height', totalHeight);
				});
			}, 100);
		};
		autoHeight();


		// Click tab menu
		$('.sc-tab-nav a').on('click', function (event) {
			var $this = $(this),
				tab = $this.data('tab');
			$('.tab-content')
				.addClass('animated-fast fadeOutDown');
			$('.tab-content')
				.removeClass('active');
			$('.sc-tab-nav li').removeClass('active');
			$this
				.closest('li')
				.addClass('active')
			$this
				.closest('.sc-tabs')
				.find('.tab-content[data-tab-content="' + tab + '"]')
				.removeClass('animated-fast fadeOutDown')
				.addClass('animated-fast active fadeIn');
			autoHeight();
			event.preventDefault();
		});
	};


	var goToTop = function () {
		$('.js-gotop').on('click', function (event) {
			event.preventDefault();
			$('html, body').animate({
				scrollTop: $('html').offset().top
			}, 500, 'easeInOutExpo');
			return false;
		});
		$(window).scroll(function () {
			var $win = $(window);
			if ($win.scrollTop() > 200) {
				$('.js-top').addClass('active');
			} else {
				$('.js-top').removeClass('active');
			}
		});
	};


	// Loading page
	var loaderPage = function () {
		$(".sc-loader").fadeOut("slow");
	};

	var counter = function () {
		$('.js-counter').countTo({
			formatter: function (value, options) {
				return value.toFixed(options.decimals);
			},
		});
	};

	var counterWayPoint = function () {
		if ($('#sc-home-statistics').length > 0) {
			$('#sc-home-statistics').waypoint(function (direction) {

				if (direction === 'down' && !$(this.element).hasClass('animated')) {
					setTimeout(counter, 400);
					$(this.element).addClass('animated');
				}
			}, {
				offset: '90%'
			});
		}
	};

	$(function () {
		mobileMenuOutsideClick();
		offcanvasMenu();
		burgerMenu();
		contentWayPoint();
		dropdown();
		tabs();
		goToTop();
		loaderPage();
		counterWayPoint();
	});
}());


//
//Partner Carousel Controls
//
$(document).ready(() => {
	$('.partners-carousel').slick({
		autoplay: true,
		autoplaySpeed: 5000,
		infinite: true,
		speed: 300,
		arrows: true,
		slidesToShow: 4,
		slidesToScroll: 1,
        dots: false,
		responsive: [{
			breakpoint: 1024,
			settings: {
				slidesToShow: 3,
				slidesToScroll: 3,
				infinite: true
			}
		},
		{
			breakpoint: 600,
			settings: {
				slidesToShow: 1,
				slidesToScroll: 1
			}
		},
		]
	});
});


//
//Coach Carousel Controls
//
$(document).ready(() => {
	$('.coaches-carousel').slick({
		autoplay: true,
		autoplaySpeed: 5000,
		infinite: true,
		speed: 1000,
		arrows: true,
		slidesToShow: 4,
		slidesToScroll: 4,
		dots: true,
		responsive: [{
			breakpoint: 1024,
			settings: {
				slidesToShow: 3,
				slidesToScroll: 3,
				infinite: true,
				dots: false
			}
		},
		{
			breakpoint: 600,
			settings: {
				slidesToShow: 1,
				slidesToScroll: 1,
                dots: false
			}
		},
		]
	});
});


//
//Coach Carousel Controls
//
$(document).ready(() => {
	$('.about-carousel').slick({
		autoplay: true,
		autoplaySpeed: 5000,
		infinite: true,
		speed: 1000,
		arrows: true,
		slidesToShow: 1,
		slidesToScroll: 1,
		lazyLoad: 'ondemand',
	});
});


//
//Coach Card Controls
//
function flip() {
	$('.coach-card').toggleClass('flipped');
}




var counter = function () {
	$('.js-counter').countTo({
		formatter: function (value, options) {
			return value.toFixed(options.decimals);
		},
	});
};

var counterWayPoint = function () {
	if ($('#sc-home-statistics').length > 0) {
		$('#sc-home-statistics').waypoint(function (direction) {

			if (direction === 'down' && !$(this.element).hasClass('animated')) {
				setTimeout(counter, 400);
				$(this.element).addClass('animated');
			}
		}, {
			offset: '90%'
		});
	}
};


$(window).scroll(function () {
	if ($(document).scrollTop() > 60) {
		$('nav').addClass('shrink');
		$('nav').addClass('sticky')
	} else {
		$('nav').removeClass('shrink');
		$('nav').removeClass('sticky')
	}
});