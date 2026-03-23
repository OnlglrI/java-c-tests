package com.example.shop.stats;

import com.example.shop.model.Order;
import com.example.shop.repository.OrderRepository;
import com.example.shop.repository.ProductRepository;
import com.example.shop.repository.UserRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import java.math.BigDecimal;
import java.util.*;
import java.util.stream.Collectors;

@Service
@RequiredArgsConstructor
public class StatsService {

    private final UserRepository userRepository;
    private final ProductRepository productRepository;
    private final OrderRepository orderRepository;

    public Map<String, Object> getSummary() {
        long totalUsers = userRepository.count();
        long totalProducts = productRepository.count();
        long totalOrders = orderRepository.count();

        BigDecimal totalRevenue = orderRepository.findAll().stream()
                .map(Order::getTotalPrice)
                .reduce(BigDecimal.ZERO, BigDecimal::add);

        Map<String, Object> summary = new LinkedHashMap<>();
        summary.put("totalUsers", totalUsers);
        summary.put("totalProducts", totalProducts);
        summary.put("totalOrders", totalOrders);
        summary.put("totalRevenue", totalRevenue);
        return summary;
    }

    public List<Map<String, Object>> getTopProducts() {
        List<Order> orders = orderRepository.findAll();

        Map<Long, Long> productCounts = orders.stream()
                .collect(Collectors.groupingBy(
                        o -> o.getProduct().getId(),
                        Collectors.counting()
                ));

        Map<Long, String> productNames = orders.stream()
                .collect(Collectors.toMap(
                        o -> o.getProduct().getId(),
                        o -> o.getProduct().getName(),
                        (a, b) -> a
                ));

        return productCounts.entrySet().stream()
                .sorted(Map.Entry.<Long, Long>comparingByValue().reversed())
                .limit(5)
                .map(entry -> {
                    Map<String, Object> item = new LinkedHashMap<>();
                    item.put("productId", entry.getKey());
                    item.put("productName", productNames.getOrDefault(entry.getKey(), ""));
                    item.put("orderCount", entry.getValue());
                    return item;
                })
                .collect(Collectors.toList());
    }
}
